using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace PlaytraGamesLtd
{
	public class Utils
	{

        public static float GetAngleInDegToPoint(Vector2 finalPos)
        {
            float val = Mathf.Pow(Mathf.Abs(finalPos.x), 2) + Mathf.Pow(Mathf.Abs(finalPos.y), 2);
            float Hypo = Mathf.Sqrt(val);
            float sin = (Mathf.Abs(finalPos.x) > Mathf.Abs(finalPos.y) ? Mathf.Abs(finalPos.x) : Mathf.Abs(finalPos.y)) / Hypo;

            if (Mathf.Abs(finalPos.x) > Mathf.Abs(finalPos.y))
            {
                if (finalPos.x > 0)
                {
                    if (finalPos.y > 0)
                    {
                        return (Mathf.Acos(sin) * 180) / Mathf.PI;
                    }
                    else
                    {
                        return 360 - ((Mathf.Acos(sin) * 180) / Mathf.PI);
                    }
                }
                else
                {
                    if (finalPos.y > 0)
                    {
                        return 180 - ((Mathf.Acos(sin) * 180) / Mathf.PI);
                    }
                    else
                    {
                        return 180 + ((Mathf.Acos(sin) * 180) / Mathf.PI);
                    }
                }
            }
            else
            {
                if (finalPos.y > 0)
                {
                    if (finalPos.x > 0)
                    {
                        return 90 - (Mathf.Acos(sin) * 180) / Mathf.PI;
                    }
                    else
                    {
                        return 90 + ((Mathf.Acos(sin) * 180) / Mathf.PI);
                    }
                }
                else
                {
                    if (finalPos.x > 0)
                    {
                        return 270 + ((Mathf.Acos(sin) * 180) / Mathf.PI);
                    }
                    else
                    {
                        return 270 - ((Mathf.Acos(sin) * 180) / Mathf.PI);
                    }
                }
            }
        }

        public static void Serialize<T>(T obj, string path)
		{
			var serializer = new XmlSerializer(obj.GetType());
			using (var writer = XmlWriter.Create(path))
			{
				serializer.Serialize(writer, obj);
			}
		}

        public static string SerializeToString<T>(T obj)
        {
            var serializer = new XmlSerializer(obj.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, obj);
                return textWriter.ToString();
            }
        }

        public static byte[] SerializeToByteWithVersion<T>(T obj, SavingVersionType v)
        {
            var serializer = new XmlSerializer(obj.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, obj);
                return Encoding.ASCII.GetBytes(v.ToString() + "%&" + textWriter.ToString());
            }
        }

        public static byte[] SerializeToByte<T>(T obj)
        {
            var serializer = new XmlSerializer(obj.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, obj);
                return Encoding.ASCII.GetBytes(textWriter.ToString());
            }
        }

        public static void SerializeEncoded<T>(T obj, string path, bool indent, Encoding encoding)
        {
            var serializer = new XmlSerializer(obj.GetType());
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = indent;
            setting.Encoding = encoding;
            using (var writer = XmlWriter.Create(path, setting))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public static void SerializeEncodedBase64<T>(T obj, string path)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            StringBuilder s = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(s);
            serializer.Serialize(writer, obj);

            var plainTextBytes = Encoding.UTF8.GetBytes(s.ToString());
            string base64 = Convert.ToBase64String(plainTextBytes);

            using (StreamWriter writetext = new StreamWriter(path))
            {
                writetext.Write(base64);
            }

        }


        public static T DeserializeEncodedBase64<T>(string file) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamReader reader = new StreamReader(file))
            {
                string r = reader.ReadToEnd();
                var base64EncodedBytes = Convert.FromBase64String(r);
                string res = Encoding.UTF8.GetString(base64EncodedBytes);
                using (TextReader stringReader = new StringReader(res))
                {
                    return serializer.Deserialize(stringReader) as T;
                }
            }
        }

        public static T Deserialize<T>(string file) where T : class
		{
			var serializer = new XmlSerializer(typeof(T));
			using (StreamReader reader = new StreamReader(file))
			{
				return serializer.Deserialize(reader) as T;
			}
		}

        public static Task<T> DeserializeObjectAsync<T>(string Value) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new System.IO.StringReader(Value))
            {
                T theObject = (T)serializer.Deserialize(reader);
                return Task.FromResult(theObject);
            }
        }

        public static T DeserializeFromString<T>(string Value) where T : class
		{
			var serializer = new XmlSerializer(typeof(T));
			using (StringReader reader = new System.IO.StringReader(Value))
			{
				return serializer.Deserialize(reader) as T;
			}
		}
        public static T DeserializeFromStringE<T>(string Value) where T : new()
        {
            var serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new System.IO.StringReader(Value))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static T DeserializeStreamingAssetsCSV<T>(string path) where T : new()
		{
			List<string> stringList = new List<string>();
			StreamReader inp_stm = new StreamReader(path);
            while (!inp_stm.EndOfStream)
            {
                string inp_ln = inp_stm.ReadLine();
                stringList.Add(inp_ln);
            }
            inp_stm.Close();

			return DeserializerCSVtoLevelStorageClass<T>(stringList);
		}

		public static T DeserializeResourcesCSV<T>(string path) where T : new()
        {

			TextAsset PrnFile = Resources.Load(path) as TextAsset;
            List<string> stringList = new List<string>();
			stringList.AddRange(PrnFile.text.Split('\n').ToList());
			stringList.Remove(stringList.Last());
			return DeserializerCSVtoLevelStorageClass<T>(stringList);
        }

		public static T DeserializerCSVtoLevelStorageClass<T>(List<string> stringList) where T : new()
		{
			T res = new T();
            for (int i = 1; i < stringList.Count; i++)
            {
               
            }
			return res;
		}


        public static void WriteLog(string Log)
        {
            string path;
            path = System.IO.Path.Combine(Application.dataPath, "Log.txt");
            try
            {
                using (TextWriter outputFile = new StreamWriter(path, true))
                {
                    outputFile.WriteLine(Log);
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("Error  1 " + ex.Message + ex.Source);
            }
        }

        public static void WriteLog(string Log, Color c)
        {
            string path;
            Log = Environment.NewLine + Log;
            path = System.IO.Path.Combine(Application.dataPath, "Log.html");
            try
            {
                using (TextWriter outputFile = new StreamWriter(path, true))
                {
                    //GameManagerScript
                    if (c == Color.red)
                    {
                        outputFile.WriteLine("<color=#ff0000ff>" + Log + "</color>");
                    }
                    //CharacterBase
                    if (c == Color.green)
                    {
                        outputFile.WriteLine("<color=#00ff00ff>" + Log + "</color>");
                    }
                    //AI
                    if (c == Color.blue)
                    {
                        outputFile.WriteLine("<color=#00ffffff>" + Log + "</color>");
                    }
                    //UI/UserInput
                    if (c == Color.yellow)
                    {
                        outputFile.WriteLine("<color=#ffff00ff>" + Log + "</color>");
                    }
                    //Info
                    if (c == Color.black)
                    {
                        outputFile.WriteLine(Log);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("Error  1 " + ex.Message + ex.Source);
            }

#if UNITY_ANDROID && !UNITY_EDITOR
        path = System.IO.Path.Combine(Application.persistentDataPath, "Log.txt");
        Debug.Log("Enter");
        try
        {
            using (TextWriter outputFile = new StreamWriter(path, true))
            {
                //GameManagerScript
                if (c == Color.red)
                {
                    outputFile.WriteLine("<color=#ff0000ff>" + Log + "</color>");
                }
                //CharacterBase
                if (c == Color.green)
                {
                    outputFile.WriteLine("<color=#00ff00ff>" + Log + "</color>");
                }
                //AI
                if (c == Color.blue)
                {
                    outputFile.WriteLine("<color=#00ffffff>" + Log + "</color>");
                }
                //UI/UserInput
                if (c == Color.yellow)
                {
                    outputFile.WriteLine("<color=#ffff00ff>" + Log + "</color>");
                }
                //Info
                if (c == Color.black)
                {
                    outputFile.WriteLine(Log);
                }
            }
        }
        catch(System.Exception ex)
        {
        Debug.Log("Error  1 " + ex.Message + ex.Source);
        }
#elif UNITY_IOS && !UNITY_EDITOR

        path = System.IO.Path.Combine(Application.persistentDataPath, "Log.txt");
        try
        {
            
        Debug.Log("FileStream");
            //GameManagerScript
            if (c == Color.red)
            {
                File.AppendAllText(path, "<color=#ff0000ff>" + Log + "</color>");
            }
            //CharacterBase
            if (c == Color.green)
            {
                File.AppendAllText(path,"<color=#00ff00ff>" + Log + "</color>");
            }
            //AI
            if (c == Color.blue)
            {
                File.AppendAllText(path,"<color=#00ffffff>" + Log + "</color>");
            }
            //UI/UserInput
            if (c == Color.yellow)
            {
                File.AppendAllText(path,"<color=#ffff00ff>" + Log + "</color>");
            }

            //Info
            if (c == Color.black)
            {
                File.AppendAllText(path,Log);
            }
           
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error  0 " + ex.Message + ex.Source);
        }


#endif


        }

        public static void ClearLog()
        {
            string path;
#if UNITY_EDITOR
            path = Application.dataPath + "Log.txt";
#elif UNITY_ANDROID && !UNITY_EDITOR
path = System.IO.Path.Combine(Application.persistentDataPath, "Log.txt");
        try
        {
            using (StreamWriter outputFile = new StreamWriter(path, true))
            {
                outputFile.WriteLine("");
            }
        }
        catch(System.Exception ex)
        {
        Debug.Log("Error  0 " + ex.Message + ex.Source);
        }
#elif UNITY_IOS && !UNITY_EDITOR
        path = System.IO.Path.Combine(Application.persistentDataPath, "Log.txt");
        try
        {
            byte[] info = new UTF8Encoding(true).GetBytes("");
            Debug.Log("Clean 1 ");
            File.WriteAllBytes(path, info);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error  0 " + ex.Message + ex.Source);
        }
#endif


        }

        public static bool CheckStatsValues(Vector2 valuesToCheck, ValueCheckerType typeOfCheck, float current)
        {
            switch (typeOfCheck)
            {
                case ValueCheckerType.LessThan:
                    if (current < valuesToCheck.x)
                    {
                        return true;
                    }
                    break;
                case ValueCheckerType.EqualTo:
                    if (current == valuesToCheck.x)
                    {
                        return true;
                    }
                    break;
                case ValueCheckerType.MoreThan:
                    if (current > valuesToCheck.x)
                    {
                        return true;
                    }
                    break;
                case ValueCheckerType.Between:
                    valuesToCheck = valuesToCheck.x > valuesToCheck.y ? new Vector2(valuesToCheck.x, valuesToCheck.y) : new Vector2(valuesToCheck.y, valuesToCheck.x);
                    if (current <= valuesToCheck.x && current >= valuesToCheck.y)
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }


#if UNITY_XBOXONE || UNITY_XBOX360

        public static void SaveData<T>(T data)
        {

        }
#endif

#if UNITY_SWITCH

        public static void SaveData<T>(T data)
        {

        }
#endif

#if UNITY_PC
        public static void SaveData<T>(T data)
        {

        }
#endif

    }

    public static class Extentions
    {
        static string temp_String;
        public static string FillGapsToLeft(this string value, int maxSize, char gapFillCharacter)
        {
            temp_String = "";
            for (int i = 0; i < maxSize || i < value.Length; i++)
            {
                temp_String = temp_String.Insert(0, i < value.Length ? value[value.Length - 1 - i].ToString() : gapFillCharacter.ToString());
            }
            return temp_String;
        }
    }


#region Checks

    public enum ValueCheckerType
    {
        LessThan,
        EqualTo,
        MoreThan,
        Between
    }

#endregion
}


