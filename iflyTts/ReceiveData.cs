using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iflyTts
{
    internal class ReceiveData
    {
        /// <summary>
        /// 0:成功
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 本次会话的ID，只在第一帧请求时返回
        /// </summary>
        public string? Sid { get; set;}

        /// <summary>
        /// data可能返回null
        /// </summary>
        public Data? Data { get; set; }

    }
    class Data
    {
        /// <summary>
        /// 合成后的音频片段,采用base64编码
        /// </summary>
        public string? Audio { get; set; }

        /// <summary>
        /// 合成进度,指当前合成文本的字节数
        /// 注：请注意合成是以句为单位切割的，若文本只有一句话，则每次返回结果的ced是相同的。
        /// </summary>
        public string? Ced { get; set; }

        /// <summary>
        /// 当前音频流状态，1表示合成中，2表示合成结束
        /// </summary>
        public int Status { get; set; }
    }
}
