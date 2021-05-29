using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using MsgPack;
using MsgPack.Serialization;
using Qrame.CoreFX.ExtensionMethod;

namespace Qrame.CoreFX.Data.MsgPack
{
    public class MsgPackFormat
    {
        public byte[] Serialize<T>(T thisObj)
        {
            var serializer = MessagePackSerializer.Get<T>();

            using (var byteStream = new MemoryStream())
            {
                serializer.Pack(byteStream, thisObj);
                return byteStream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] bytes)
        {
            var serializer = MessagePackSerializer.Get<T>();
            using (var byteStream = new MemoryStream(bytes))
            {
                return serializer.Unpack(byteStream);
            }
        }
    }
}
