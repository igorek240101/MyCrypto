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

        delegate int CryptoMeth(string key, string text, int i, int j);

        [HttpPost]
        [Route("Decrypt/{key}")]
        public FileResult DecryptAsync(string key, IFormFile file)
        {

            if (key.All(t => Model.VizSquere.Contains(char.ToUpper(t))))
            {
                if (file.FileName.Split('.')[^1] == "txt")
                {
                    byte[] array = Crypt(key, Encoding.UTF8.GetString(new BinaryReader(file.OpenReadStream()).ReadBytes((int)file.Length)), Decrypt);
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
        public FileResult Encrypt(string key, IFormFile file)
        {
            if (key.All(t => Model.VizSquere.Contains(char.ToUpper(t))))
            {
                if (file.FileName.Split('.')[^1] == "txt")
                {
                    byte[] array = Crypt(key, Encoding.UTF8.GetString(new BinaryReader(file.OpenReadStream()).ReadBytes((int)file.Length)), Encrypt);
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

        private byte[] Crypt(string key, string s, CryptoMeth meth)
        {
            string res = "";
            for (int i = 0, j = 0; i < s.Length; i++, j++)
            {
                if (Model.VizSquere.Contains(char.ToUpper(s[i])))
                {
                    int k = meth.Invoke(key, s, i, j);
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
        }

        private int Decrypt(string key, string text, int i, int j)
        {
            int delta = Model.VizSquere.ToList().IndexOf(char.ToUpper(key[j % key.Length]));
            int k = Model.VizSquere.ToList().IndexOf(char.ToUpper(text[i]));
            k -= delta;
            if (k < 0) k += Model.VizSquere.Length;
            return k;
        }

        private int Encrypt(string key, string text, int i, int j)
        {
            int delta = Model.VizSquere.ToList().IndexOf(char.ToUpper(key[j % key.Length]));
            int k = Model.VizSquere.ToList().IndexOf(char.ToUpper(text[i]));
            k += delta;
            if (k >= Model.VizSquere.Length) k -= Model.VizSquere.Length;
            return k;
        }

        private Document CryptDocx(string key, IFormFile file, CryptoMeth meth)
        {
            Stack<int> stack = new Stack<int>();
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
                        TextChange(node, key, meth);
                    }
                }
                else
                {
                    TextChange(node, key, meth);
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
            return doc;
        }

        private void TextChange(Node node, string key, CryptoMeth meth)
        {

            switch (node.NodeType)
            {
                case NodeType.Run:
                    {
                        (node as Run).Text = Encoding.UTF8.GetString(Crypt(key, (node as Run).Text, meth));
                        break;
                    }
            }

        }
    }
}
