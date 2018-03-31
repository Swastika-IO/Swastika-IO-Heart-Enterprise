﻿// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Swastika.Common.Helper
{
    /// <summary>
    /// Common helper
    /// </summary>
    public class CommonHelper
    {
        /// <summary>
        /// The base62chars
        /// </summary>
        private static readonly char[] _base62chars =
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
            .ToCharArray();

        /// <summary>
        /// The random
        /// </summary>
        private static readonly Random _random = new Random();

        /// <summary>
        /// Generates the key.
        /// </summary>
        /// <returns></returns>
        public static RSAParameters GenerateKey()
        {
            using (var key = new RSACryptoServiceProvider(2048))
            {
                return key.ExportParameters(true);
            }
        }

        /// <summary>
        /// Gets the base62.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string GetBase62(int length)
        {
            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
                sb.Append(_base62chars[_random.Next(62)]);

            return sb.ToString();
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <param name="subPaths">The sub paths.</param>
        /// <returns></returns>
        public static string GetFullPath(string[] subPaths)
        {
            string result = string.Empty;
            string strFormat = string.Empty;
            for (int i = 0; i < subPaths.Length; i++)
            {
                // TODO: Use regular string literal instead of verbatim string literal => Remove @?
                strFormat += @"{" + i + "}" + (i < subPaths.Length - 1 ? "/" : string.Empty);
            }
            return string.Format(strFormat, subPaths).Replace("//", "/");
        }

        /// <summary>
        /// Gets the random name.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static string GetRandomName(string filename)
        {
            string ext = filename.Split('.')[1];
            return string.Format("{0}.{1}", Guid.NewGuid().ToString("N"), ext);
        }

        /// <summary>
        /// Gets the web response asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<string> GetWebResponseAsync(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            using (WebResponse response = await webRequest.GetResponseAsync().ConfigureAwait(false))
            {
                using (Stream resStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="strImage64">The string image64.</param>
        /// <returns></returns>
        public static Stream LoadImage(string strImage64)
        {
            //data:image/gif;base64,
            //this image is a single pixel (black)
            try
            {
                string imgData = strImage64.Substring(strImage64.IndexOf(',') + 1);
                //byte[] bytes = Convert.FromBase64String(imgData);

                //Image image;
                //using (MemoryStream ms = new MemoryStream(bytes))
                //{
                //    image = Image.FromStream(ms);
                //}

                //return image;
                byte[] imageBytes = Convert.FromBase64String(imgData);
                // Convert byte[] to Image
                return new MemoryStream(imageBytes, 0, imageBytes.Length);
                //using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                //{
                //    Image image = Image.FromStream(ms, true);
                //    return image;
                //}
            }
            catch//(Exception ex)
            {
                return null;
            }
        }

        //private static string defaultImagePath = "http://placehold.it/200x200";
        /// <summary>
        /// Parses the name of the json property.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string ParseJsonPropertyName(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return Char.ToLower(input[0]) + input.Substring(1);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Reads from file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static string ReadFromFile(string filename)
        {
            string s = "";
            try
            {
                FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(file);
                s = sr.ReadToEnd();
                sr.Dispose();
                file.Dispose();
            }
            catch
            {
                s = "";
            }
            return s;
        }

        /// <summary>
        /// Removes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static bool RemoveFile(string filePath)
        {
            bool result = false;
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    result = true;
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// Saves the file base64.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="strBase64">The string base64.</param>
        /// <returns></returns>
        public static bool SaveFileBase64(string folder, string filename, string strBase64)
        {
            //data:image/gif;base64,
            //this image is a single pixel (black)
            try
            {
                string fullPath = GetFullPath(new string[]
                {
                    folder,
                    filename
                });
                string fileData = strBase64.Substring(strBase64.IndexOf(',') + 1);
                byte[] bytes = Convert.FromBase64String(fileData);

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                FileStream fs = new FileStream(fullPath, FileMode.Create);
                BinaryWriter w = new BinaryWriter(fs);
                try
                {
                    w.Write(bytes);
                }
                finally
                {
                    fs.Close();
                    w.Close();
                }
                return true;
            }
            catch//(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Uploads the file asynchronous.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static string UploadFile(
           string fullPath, FileInfo file)
        {

            try
            {
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                if (file != null)
                {
                    //var fileName = ContentDispositionHeaderValue.Parse
                    //    (file.ContentDisposition).FileName.Trim('"');
                    string fileName = string.Format("{0}.{1}",
                        Guid.NewGuid().ToString("N"),
                        file.FullName.Split('.').Last());
                    using (var fileStream = new FileStream(Path.Combine(fullPath, fileName), FileMode.Create, FileAccess.ReadWrite))
                    {
                        file.CopyTo(Path.Combine(fullPath, fileName));
                        return fileName;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// Writes the bytes to file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="strBase64">The string base64.</param>
        public static void WriteBytesToFile(string fullPath, string strBase64)
        {
            string fileData = strBase64.Substring(strBase64.IndexOf(',') + 1);
            byte[] bytes = Convert.FromBase64String(fileData);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            FileStream fs = new FileStream(fullPath, FileMode.Create);
            BinaryWriter w = new BinaryWriter(fs);
            try
            {
                w.Write(bytes);
            }
            finally
            {
                fs.Close();
                w.Close();
            }
        }

        //TODO: Still need?
        //public static string UploadPhoto(string fullPath, Image img)
        //{
        //    try
        //    {
        //        if (!Directory.Exists(fullPath))
        //        {
        //            Directory.CreateDirectory(fullPath);
        //        }

        //        if (img != null)
        //        {
        //            //string fileExt = GetFilenameExtension(img.RawFormat);
        //            //file_name = (guid + fileExt).Trim();
        //            //file_dir = filePath + file_name;
        //            //ImageResizer.ResizeStream(TTXConstants.Params.photoSize, img, file_dir);

        //            return ImageHelper.ResizeImage(img, fullPath);
        //        }
        //    }
        //    catch (Exception ex) // TODO: Add more specific exeption types instead of Exception only
        //    {
        //        return string.Empty;
        //    }
        //    return string.Empty;
        //}
    }
}
