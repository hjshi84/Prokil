using System;
using System.Collections.Generic;
using System.Text;

namespace Prokil
{
    public class Valueitem
    {
        private string _time;
        public string time
        {
            get
            {
                return _time;
            }
        }
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
        }
        private string _fromtime;
        public string fromtime
        {
            get
            {
                return _fromtime;
            }
        }
        private string _totime;
        public string totime
        {
            get
            {
                return _totime;
            }
        }
        public Valueitem(string ft, string tt, string n, string t) 
        { 
            _fromtime=ft;
            _totime=tt;
            _name=n;
            _time=t;

        } 
    }
}
