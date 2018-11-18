using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;

using System.IO;
using System.Security.Cryptography;

public class xflyOCR : MonoBehaviour {
    private string url= "http://webapi.xfyun.cn/v1/service/v1/ocr/handwriting";
    private string apikey = "5b89e42723726c0cf16dbfaf63b9ecac";
    private string appid = "5b1e612d";
    private string curTime;
    private string param;
    private string checkSum;
    public bool postdone;
    public string result;
    
    // Use this for initialization
    void Start () {
        curTime = GetTimeStamp().ToString();


        param = Base64Encode("{\"location\":\"true\",\"language\":\"en\"}");

        checkSum = UserMd5(apikey + curTime + "eyJsb2NhdGlvbiI6InRydWUiLCJsYW5ndWFnZSI6ImVuIn0=");
                                                

        Debug.Log("curTime:" + curTime);
        //Debug.Log("json data:" + data.ToJson().ToString());
        Debug.Log("param:" + param);
        Debug.Log("checksum:" + checkSum);

       // JsonData image = new JsonData();
       // image["image"] = ImgToBase64("E:\\Unity Project\\gpMath2\\Assets\\Images\\Screenshot.png");
        // string imagedata = WWW.EscapeURL("{ \'image\': "+ ImgToBase64("E:\\Unity Project\\gpMath2\\Assets\\Images\\Screenshot.png")+"}");
        //string imagedata = WWW.EscapeURL("image=" + ImgToBase64("E:\\Unity Project\\gpMath2\\Assets\\Images\\Screenshot.png"));
       // string imagedata = "image=" + WWW.EscapeURL(ImgToBase64("E:\\Unity Project\\gpMath2\\Assets\\Images\\Screenshot.png"));
        //Debug.Log(image.ToJson().ToString());
        Debug.Log(parseResult("v{\"code\":\"0\",\"data\":{\"block\":[{\"type\":\"text\",\"line\":[{\"confidence\":1,\"location\":{\"top_left\":{\"x\":404,\"y\":408},\"right_bottom\":{\"x\":1032,\"y\":647}},\"word\":[{\"content\":\"y\"},{\"content\":\"=\"},{\"content\":\"2X\"},{\"content\":\"t\"},{\"content\":\"I\"}]}]}]},\"desc\":\"success\",\"sid\":\"wcr0000919c@gza9470e8d9cfa460b00\"}"));

        

        //Debug.Log()

        //Debug.Log(ImgToURLString("Assets\\Images\\test.jpg"));

       // StartCoroutine(sendPost());
    }
    
    // Update is called once per frame
    void Update () {
		
	}

    public void getOCR(Texture2D tex) {
        StartCoroutine(sendPost(tex));
    }
    

    IEnumerator sendPost(Texture2D tex) {
        curTime = GetTimeStamp().ToString();
        param = Base64Encode("{\"location\":\"true\",\"language\":\"en\"}");

        checkSum = UserMd5(apikey + curTime + param);

        Dictionary<string, string> header = new Dictionary<string, string>();
        
        header.Add("X-Appid", appid);
        header.Add("X-CurTime", curTime);
        header.Add("X-Param", param);
        header.Add("X-CheckSum", checkSum);

       // JsonData image = new JsonData();
      //  image["image"] = ImgToBase64("E:\\Unity Project\\gpMath2\\Assets\\Images\\Screenshot.png");
        string imagedata = "image=" + WWW.EscapeURL( ImgToBase64(tex));
        

        byte[] postBytes = System.Text.Encoding.GetEncoding(0).GetBytes(imagedata);

        WWW www = new WWW(url, postBytes, header);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
        }
        Debug.Log(www.text);
        result = parseResult(www.text);
        postdone = true;
    }

    public string parseResult(string s) {
        int index = 0;
        string ret="";
        while ((index=s.IndexOf("content") )!= -1) {
            index += 10;
            s=s.Substring(index, s.Length - index);
            int indexnext = s.IndexOf("\"");
            string tmp= s.Substring(0, indexnext);
            ret += tmp;
            s = s.Substring(indexnext, s.Length - indexnext);
        }
        return beautifyResult( ret);
    }

    public string beautifyResult(string s)
    {
        string ret="";
        foreach (char c in s) {
            int tc = (int)c;
            char cc=c;
            if (cc =='I') { cc = '1'; ret = ret+cc; continue; }
            if (tc > 'A' &&tc < 'Z') {
                tc = (int)tc - (int)('A' - 'a');
                cc = (char)tc;
            }
            ret += cc;
            
        }
        return ret;
    }

    public string getResult() {
        if (postdone) {
            postdone = false;
            return result;
        }
        return null;
    }

    public static long GetTimeStamp(bool bflag = true)
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long ret;
        if (bflag)
            ret = Convert.ToInt64(ts.TotalSeconds);
        else
            ret = Convert.ToInt64(ts.TotalMilliseconds);
        return ret;
    }

    public static string Base64Encode(string message)
    {
        byte[] bytes = Encoding.GetEncoding(0).GetBytes(message);
        return Convert.ToBase64String(bytes);
    }

    public static string md5(string source)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }

    //public string ImgToBase64(string path)
    //{
        
    //    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
    //    byte[] buffer = new byte[fs.Length];
    //    fs.Read(buffer, 0, (int)fs.Length);
    //    string base64String = Convert.ToBase64String(buffer);
    //   // Debug.Log("获取当前图片base64为---" + base64String);
    //   // base64String = Uri.EscapeDataString(base64String);
    //    return  base64String;
       
    //}
    public string ImgToBase64(Texture2D tex)
    {

        //FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        string base64String = Convert.ToBase64String(tex.EncodeToPNG());
        // Debug.Log("获取当前图片base64为---" + base64String);
        // base64String = Uri.EscapeDataString(base64String);
        return base64String;

    }

    static string UserMd5(string strToEncrypt)
    {
        byte[] bs = UTF8Encoding.UTF8.GetBytes(strToEncrypt);
        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();

        byte[] hashBytes = md5.ComputeHash(bs);

        string hashString = "";
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashString.PadLeft(32, '0');
    }
}
