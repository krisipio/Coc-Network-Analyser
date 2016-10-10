using System;
using Newtonsoft.Json.Linq;

namespace coc_messages_csharp
{
    public class Decoder
    {
        private JObject definitions;
        private int bitmask = 0;

        public Decoder()
        {
            this.definitions = Definitions.read();
        }

        public Decoder(JObject definitions)
        {
            this.definitions = definitions;
        }

        public void ReloadDefinitions()
        {
            this.definitions = Definitions.read();
        }

        public JObject decode(int messageId, int unknown, byte[] payload)
        {
            JObject decoded = new JObject();
            JObject definition = (JObject)definitions.GetValue(messageId.ToString());
            if (definition != null)
            {
                Reader reader = new Reader(messageId, unknown, payload);
                decoded.Add("name", definition["name"]);
                if (definition["fields"] != null)
                    decoded.Add("fields", this.decodeFields(reader, (JArray)definition["fields"]));
                if (this.bitmask != 0)
                {
                    reader.skipBytes(1);
                    this.bitmask = 0;
                }
                if (reader.availableBytes() > 0)
                    throw new ArgumentOutOfRangeException("Unused buffer remains.");
            }
            else throw new ArgumentOutOfRangeException(String.Format("{0} has not been defined.", messageId));
            return decoded;
        }

        private JObject decodeFields(Reader reader, JArray fields)
        {
            JObject decoded = new JObject();
            int count = fields.Count;
            for (int i = 0; i < count; ++i)
            {
                JObject field = (JObject)fields[i];
                if (field["name"] == null)
                    field["name"] = String.Format("unknown_{0}", i.ToString().PadLeft((int)Math.Floor(Math.Log10(count)), '0'));
                decoded.Add((string)field["name"], this.decodeField(reader, (string)field["type"]));
            }
            return decoded;
        }

        private JToken decodeField(Reader reader, string type)
        {
            if (reader.availableBytes() == 0)
                throw new ArgumentOutOfRangeException("Read buffer out of data.");
            if (type.Substring(0, 1) == "?")
            {
                if (this.bitmask > 0)
                {
                    this.bitmask = (this.bitmask << 1) % 16;
                    if (this.bitmask == 0)
                        return null;
                }
                else
                    this.bitmask = 1;
                if ((reader.peekInt(1) & this.bitmask) != 0)
                {
                    type = type.Substring(1);
                    reader.skipBytes(1);
                    this.bitmask = 0;
                }
                else
                    return null;
            }
            else if (this.bitmask != 0)
            {
                reader.skipBytes(1);
                this.bitmask = 0;
            }
            if (type.Contains("["))
            {
                int position = type.IndexOf('[');
                int count;
                if (position < type.Length - 2)
                    count = Int32.Parse(type.Substring(position + 1, type.Length - position - 2));
                else
                    count = reader.readInt();
                JArray decoded = new JArray();
                for (int i = 0; i < count; ++i)
                    decoded.Add(this.decodeField(reader, type.Substring(0, position)));
                return decoded;
            }
            else
            {
                if (type == "BOOLEAN")
                {
                    if (reader.readInt(1) != 0)
                        return true;
                    else
                        return false;
                }
                else if (type == "BYTE")
                    return reader.readByte();
                else if (type == "INT")
                    return reader.readInt();
                else if (type == "LONG")
                    return reader.readLong();
                else if (type == "STRING")
                    return reader.readString();
                else if (type == "ZIP_STRING")
                    return JObject.Parse(reader.readZString());
                else if (this.definitions["component"][type] != null)
                {
                    JObject decoded = this.decodeFields(reader, (JArray)this.definitions["component"][type]["fields"]);
                    if (this.definitions["component"][type]["extensions"] != null)
                    {
                        if (this.definitions["component"][type]["extensions"][decoded["id"]] == null)
                            throw new ArgumentOutOfRangeException(String.Format("{0}(id={1}) has not yet been implemented.", type, decoded["id"]));
                    }
                    return decoded;
                }
                else throw new ArgumentOutOfRangeException(String.Format("{0} has not yet been implemented.", type));
            }
        }
    }
}
