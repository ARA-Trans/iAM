using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RoadCareGlobalOperations
{
    static public class AssemblySerialize
    {
        /// <summary>
        /// Create a byte array of serializable object for input into database BINARY field. File temp.dat created in process.
        /// </summary>
        /// <param name="objectToSerialize">Object to serialize</param>
        /// <returns>Byte array of serialized object</returns>
        static public byte[] SerializeObjectToByteArray(object objectToSerialize)
        {
            if (objectToSerialize == null) return null;
			MemoryStream memStream = new MemoryStream();
            BinaryFormatter bFormatter = new BinaryFormatter();
			bFormatter.Serialize(memStream, objectToSerialize);
			memStream.Seek(0, SeekOrigin.Begin);
			//objectToSerialize = bFormatter.Deserialize(memStream);
			byte[] byteArray = new byte[memStream.Length];
			byteArray = memStream.ToArray();
			memStream.Close();
            return byteArray;
        }

        /// <summary>
        /// Deserialize a byte array back to object.
        /// </summary>
        /// <param name="assembly">Byte array created from BINARY database field</param>
        /// <returns>Deserialized object</returns>
        static public object DeSerializeObjectFromByteArray(byte[] assembly)
        {
			MemoryStream memStream = new MemoryStream();
			memStream.Write(assembly, 0, assembly.Length);
			memStream.Seek(0, SeekOrigin.Begin);
			object objectToSerialize;
            BinaryFormatter bFormatter = new BinaryFormatter();
            objectToSerialize = bFormatter.Deserialize(memStream);
			memStream.Close();
            return objectToSerialize;
        }
	}
}
