﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class LoginData_mod
    {
        private int user_id;

        private string user_password = "";

        private int uid;

        private short type;

        public short Type
        {
            get { return type; }
            set { type = value; }
        }


        public int User_id
        {
            get
            {
                return user_id;
            }

            set
            {
                user_id = value;
            }
        }

        public string User_password
        {
            get
            {
                return user_password;
            }

            set
            {
                user_password = value;
            }
        }

        public int Uid
        {
            get
            {
                return uid;
            }

            set
            {
                uid = value;
            }
        }
    }
}
