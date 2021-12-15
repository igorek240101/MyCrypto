using Aspose.Words;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCryptoAPI.Controllers
{
    [ApiController]
    [Route("Crypto")]
    public class CryptoController : ControllerBase
    {
#if TEST
        public delegate int CryptoMeth(char key, char text);
#else
        private delegate int CryptoMeth(char key, char text);
#endif

        [HttpPost]
        [Route("Decrypt/{key}")]
        public async Task<FileResult> DecryptAsync(string key, IFormFile file)
        {

            if (key.All(t => Model.VizSquere.Contains(char.ToUpper(t))))
            {
                if (file.FileName.Split('.')[^1] == "txt")
                {
                    byte[] array = await Crypt(key, Encoding.UTF8.GetString(new BinaryReader(file.OpenReadStream()).ReadBytes((int)file.Length)), Decrypt);
                    return File(array, "application/txt", "res.txt");
                }
                else
                {
                    if (file.FileName.Split('.')[^1] == "docx")
                    {
                        Document doc = CryptDocx(key, file, Decrypt);
                        MemoryStream dstStream = new MemoryStream();
                        doc.Save(dstStream, SaveFormat.Docx);
                        FileContentResult fileContent = new FileContentResult(dstStream.ToArray(), "application/docx");
                        fileContent.FileDownloadName = "res.docx";
                        return fileContent;
                    }
                    else return null;
                }
            }
            else
            {
                return File(new byte[] { }, "application/txt", "res.txt");//UnprocessableEntity("Ключевое слово включает символы не входящие в русский алфавит");
            }
        }


        [HttpPost]
        [Route("Encrypt/{key}")]
        public async Task<FileResult> EncryptAsync(string key, IFormFile file)
        {
            if (key.All(t => Model.VizSquere.Contains(char.ToUpper(t))))
            {
                if (file.FileName.Split('.')[^1] == "txt")
                {
                    byte[] array = await Crypt(key, Encoding.UTF8.GetString(new BinaryReader(file.OpenReadStream()).ReadBytes((int)file.Length)), Encrypt);
                    return File(array, "application/txt", "res.txt");
                }
                else
                {
                    if (file.FileName.Split('.')[^1] == "docx")
                    {
                        Document doc = CryptDocx(key, file, Encrypt);
                        MemoryStream dstStream = new MemoryStream();
                        doc.Save(dstStream, SaveFormat.Docx);
                        FileContentResult fileContent = new FileContentResult(dstStream.ToArray(), "application/docx");
                        fileContent.FileDownloadName = "res.docx";
                        return fileContent;
                    }
                    else return null;
                }
            }
            else
            {
                return File(new byte[] { }, "application/txt", "res.txt");//UnprocessableEntity("Ключевое слово включает символы не входящие в русский алфавит");
            }
        }

#if TEST
        public static Task<byte[]> Crypt(string key, string s, CryptoMeth meth)
#else
        private static Task<byte[]> Crypt(string key, string s, CryptoMeth meth)
#endif
        {
            Task<byte[]> task = new Task<byte[]>(() =>
            {
                string res = "";
                for (int i = 0, j = 0; i < s.Length; i++, j++)
                {
                    if (Model.VizSquere.Contains(char.ToUpper(s[i])))
                    {
                        int k = meth.Invoke(key[j % key.Length], s[i]);
                        if (char.IsUpper(s[i])) res += Model.VizSquere[k];
                        else res += char.ToLower(Model.VizSquere[k]);
                    }
                    else
                    {
                        res += s[i];
                        j--;
                    }
                }
                return Encoding.UTF8.GetBytes(res);
            });
            task.Start();
            return task;
        }

#if TEST
        public static int Decrypt(char key, char text)
#else
        private static int Decrypt(char key, char text)
#endif
        {
            int delta = Model.VizSquere.ToList().IndexOf(char.ToUpper(key));
            int k = Model.VizSquere.ToList().IndexOf(char.ToUpper(text));
            k -= delta;
            if (k < 0) k += Model.VizSquere.Length;
            return k;
        }

#if TEST
        public static int Encrypt(char key, char text)
#else
        private static int Encrypt(char key, char text)
#endif
        {
            int delta = Model.VizSquere.ToList().IndexOf(char.ToUpper(key));
            int k = Model.VizSquere.ToList().IndexOf(char.ToUpper(text));
            k += delta;
            if (k >= Model.VizSquere.Length) k -= Model.VizSquere.Length;
            return k;
        }

#if TEST
        public static Document CryptDocx(string key, IFormFile file, CryptoMeth meth)
#else
        private static Document CryptDocx(string key, IFormFile file, CryptoMeth meth)
#endif
        {
            Stack<int> stack = new Stack<int>();
            List<Task> tasks = new List<Task>();
            Document doc = new Document(file.OpenReadStream());
            Node node = doc;
            stack.Push(0);
            while (stack.Count != 0)
            {
                if (node.IsComposite)
                {
                    CompositeNode composite = node as CompositeNode;
                    if (composite.ChildNodes.Count > 0)
                    {
                        stack.Push(0);
                        node = composite.FirstChild;
                        continue;
                    }
                    else
                    {
                        Task task = new Task(()=>TextChange(node, key, meth));
                        task.Start();
                        tasks.Add(task);
                    }
                }
                else
                {
                    Task task = new Task(() => TextChange(node, key, meth));
                    task.Start();
                    tasks.Add(task);
                }
                while (stack.Count != 0)
                {
                    int now = stack.Pop();
                    now++;
                    node = node.ParentNode;
                    if (node != null && (node as CompositeNode).ChildNodes.Count > now)
                    {
                        stack.Push(now);
                        node = (node as CompositeNode).ChildNodes[now];
                        break;
                    }
                }
            }
            Task.WaitAll(tasks.ToArray());
            return doc;
        }

#if TEST
        public static async void TextChange(Node node, string key, CryptoMeth meth)
#else
        private static async void TextChange(Node node, string key, CryptoMeth meth)
#endif
        {

            switch (node.NodeType)
            {
                case NodeType.Run:
                    {
                        (node as Run).Text = Encoding.UTF8.GetString(await Crypt(key, (node as Run).Text, meth));
                        break;
                    }
            }

        }
    }
}
