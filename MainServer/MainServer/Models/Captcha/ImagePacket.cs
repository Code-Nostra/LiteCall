using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using MainServer.Models.Captcha;

namespace MainServer.Models.Captcha
{
    public class ImagePacket
    {
        public string hash { get; set; } = string.Empty;
        public int len { get; set; } = 0;
        public string image { get; set; } = string.Empty;
        public ImagePacket() { }
        public ImagePacket(byte[] img_sources)
        {
            hash = ImageMethod.StringHash(img_sources);
            len = img_sources.Length;
            image = ImageMethod.EncodeBytes(img_sources);
        }
        public static ImagePacket GetImage(string code, int width, int height)
		{
			CaptchaImage captcha = new CaptchaImage(code, width, height);
			Image image = captcha.Image;
			byte[] img_byte_arr = ImageMethod.ImageToBytes(image);
			ImagePacket packet = new ImagePacket(img_byte_arr);
			return packet;
		}
        public byte[] GetRawData()
        {
            byte[] data = ImageMethod.DecodeBytes(image);

            if (data.Length != len) throw new Exception("Error data len");
            if (!ImageMethod.StringHash(data).Equals(hash)) throw new Exception("Error hash");

            return data;
        }
    }
}
