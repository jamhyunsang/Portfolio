using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

public static class Util
{
    public static string ToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public static T ToObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static bool TryParseJson<T>(string json, out T result)
    {
        try
        {
            result = JsonConvert.DeserializeObject<T>(json);
            return true;
        }
        catch (JsonException)
        {
            result = default;
            return false;
        }
    }

    public static float Lerp(float start, float end, float current, float max)
    {
        if (max == 0f)
            return start;

        float t = current / max;
        return start + (end - start) * t;
    }

    public static int Lerp(int start, int end, int current, int max)
    {
        if (max == 0)
            return start;

        float t = (float)current / max;
        return (int)(start + (end - start) * t);
    }

    public static string DeCompress(byte[] bytes)
    {
        using (var inputStream = new MemoryStream(bytes))
        {
            using (var brotliStream = new BrotliStream(inputStream, CompressionMode.Decompress))
            {
                using (var streamReader = new StreamReader(brotliStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }

    public static byte[] Decrypt(byte[] bytes)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = ClientDefine.EncryptKey;
            aes.IV = ClientDefine.EncryptIV;
            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                using (var memoryStream = new MemoryStream(bytes))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (var outputStream = new MemoryStream())
                        {
                            cryptoStream.CopyTo(outputStream);
                            return outputStream.ToArray();
                        }
                    }
                }
            }
        }
    }
}
