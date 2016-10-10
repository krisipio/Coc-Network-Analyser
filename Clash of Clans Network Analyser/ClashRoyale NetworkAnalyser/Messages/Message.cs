using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClashRoyale_NetworkAnalyser.Utils;
using Newtonsoft.Json.Linq;

namespace ClashRoyale_NetworkAnalyser.Messages
{
    public class Message
    {
        private string _name;
        private int _id;
        private string _content;
        private byte[] _rawBytes;
        private string _raw;
        private string _receivedStamp;
        private State _state;
        
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get
            {
                //try
                //{
                //    JObject decoded = _state.decoder.decode(_id, 0, _rawBytes);
                //    return _name + decoded["name"];
                //}
                //catch (Exception)
                //{
                //    //ignore
                //}

                //return _name + "Undefined";
                return _name;
            }
            set { _name = value; }
        }
        
        public string Content
        {
            get
            {
                //try
                //{
                //    JObject decoded = _state.decoder.decode(_id, 0, _rawBytes);
                //    return decoded["fields"].ToString();
                //}
                //catch (Exception)
                //{
                //    //ignore
                //}

                return _content;
            }
            set { _content = value; }
        }

        public string Raw
        {
            get { return _raw; }
            set { _raw = value; }
        }

        public string ReceivedStamp
        {
            get { return _receivedStamp; }
            set { _receivedStamp = value; }
        }

        public byte[] RawBytes
        {
            get { return _rawBytes; }
            set { _rawBytes = value; }
        }

        public Message(int id, string direction, byte[] rawBytes, int unk, State state, string content)
        {
            ID = id;
            Name = direction;
            Raw = Utilsa.BytesToString(BitConverter.GetBytes(id).Reverse().Skip(2).Concat
                    (BitConverter.GetBytes(rawBytes.Length).Reverse().Skip(1)).Concat(BitConverter.GetBytes(unk)
                    .Reverse().Skip(2)).Concat(rawBytes).ToArray());
            RawBytes = (byte[]) rawBytes;
            ReceivedStamp = DateTime.Now.ToString("HH:mm:ss.fff");
            Content = content;
            _state = state;
        }
    }
}
