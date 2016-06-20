﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTeach_DAL_Server;
using NewTeach_DAL_Data;
using Model;
using System.Collections;
using Model.Sockets;

namespace NewTeach_BLL_Server.Message
{
    internal class MessageFresh
    {
        DataPackage data;
        RefreshRequest_mod rr;

        internal MessageFresh(DataPackage dpk, int user_id)
        {
            data = dpk;
            rr = RefreshRequestConvert.ConvertToClass(dpk.Data);
            rr.User_id = user_id;
        }

        internal bool Response()
        {
            SQLService sql = new SQLService();
            List<MessageData_mod> arrMsg = sql.SelOverMessages(rr.User_id);
            Sender sender = new Sender();

            foreach (object obj in arrMsg)
            {
                MessageData_mod msg = (MessageData_mod)obj;
                byte[] dataSend = MessageDataConvert.ConvertToBytes(msg);
                DataPackage dpk = new DataPackage();
                dpk.Data = dataSend;
                dpk.Client = data.Client;
                if (!sender.SendMessage(dpk))
                    return false;
            }

            //结束标识
            DataPackage endDpk = new DataPackage();
            endDpk.Data = RefreshRequestConvert.ConvertToBytes_End(rr.Uid);
            endDpk.Client = data.Client;
            sender.SendMessage(endDpk);

            return true;
        }
    }
}
