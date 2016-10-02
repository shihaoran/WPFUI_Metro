using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMetro
{
    public class MapErrorException : ApplicationException//由用户程序引发，用于派生自定义的异常类型  
    {
        /// <summary>  
        /// 默认构造函数  
        /// </summary>  
        public MapErrorException() { }
        public MapErrorException(string message)
            : base(message) { }
        public MapErrorException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class InputStationException : ApplicationException//由用户程序引发，用于派生自定义的异常类型  
    {
        /// <summary>  
        /// 默认构造函数  
        /// </summary>  
        public InputStationException() { }
        public InputStationException(string message)
            : base(message) { }
        public InputStationException(string message, Exception inner)
            : base(message, inner) { }
    }


}
