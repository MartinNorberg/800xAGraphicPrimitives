/*
Copyright (c) 2019, Stefan Berndt

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or 
   other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, 
BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace PG2VNCViewer
{
    public class Logger
    {
        public bool invalid = true;
        public long loglevel = 1;
        public List<string> loglist = new List<string>();
        private void Log(string text, int level)
        {
            if (level <= loglevel)
            {
                lock (loglist)
                {
                    switch (level)
                    {
                        case 0:
                            loglist.Add("FATAL: " + text);
                            break;
                        case 1:
                            loglist.Add("ERROR: " + text);
                            break;
                        case 2:
                            loglist.Add("INFO: " + text);
                            break;
                        case 3:
                            loglist.Add("DEBUG: " + text);
                            break;
                    }
                    if (loglist.Count > 40) loglist.RemoveAt(0);
                    invalid = true;
                }
            }
        }
        public void Fatal(string text)
        {
            Log(text, 0);
        }
        public void Error(string text)
        {
            Log(text, 1);
        }
        public void Info(string text)
        {
            Log(text, 2);
        }
        public void Debug(string text)
        {
            Log(text, 3);
        }
    }
}
