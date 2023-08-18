using System.Text;

namespace iflyTts
{
    class SendTest
    {
        public SendTest(string appId, string text)
        {
            var sendBytes = Encoding.UTF8.GetBytes(text);
            Common = new Common()
            {
                App_Id = appId,
            };
            Business = new Business()
            {
                Aue = "raw",
                Vcn = "xiaoyan",
                Pitch = 50,
                Speed = 50,
            };
            Data = new SendData()
            {
                Status = 2,
                Text = Convert.ToBase64String(sendBytes)
            };
        }

        public Common Common { get; set; }
        public Business Business { get; set; }
        public SendData Data { get; set; }
    }

    class Common
    {
        public string? App_Id { get; set; }
    }

    class Business
    {
        public string? Aue { get; set; }

        public string? Vcn { get; set; }

        public int Pitch { get; set; }

        public int Speed { get; set; }
    }

    class SendData
    {
        public int Status { get; set; }
        public string? Text { get; set; }
    }
}
