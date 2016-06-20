﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using File_DAL;
using NewTeach_DAL_Server;
using NewTeach_DAL_Data;
using Model;
using Model.Sockets;

namespace NewTeach_BLL_Server.File
{
    internal class UploadFile
    {
        UploadFileRequest_mod rfr = new UploadFileRequest_mod();
        System.Net.Sockets.TcpClient remoteClient;
        string[] strs;
        string path;

        internal UploadFile(DataPackage data, int user_id)
        {
            rfr = FileInfoConvert.ConvertToClass_FileUpLoad(data.Data);
            rfr.User_id = user_id;
            remoteClient = data.Client;
            strs = FileCheck.CheckCreateUserDir(rfr.User_id);
            path = strs[0] + rfr.File_name;
            rfr.File_length = File_DAL.GetFileInfo.GetLength(path);
        }

        internal bool Receive()
        {
            try
            {
                string key = "";
                Random random = new Random();
                for (int i = 0; i < 16; i++)
                    key += random.Next(0, 10).ToString();

                SQLService sql = new SQLService();
                FileRequestResponse_mod frr;
                Sender sender = new Sender();

                if (sql.UpLoadFile(rfr.User_id, rfr.File_name, key, rfr.File_length, rfr.File_type))
                {
                    frr = new FileRequestResponse_mod
                    {
                        Uid = rfr.Uid,
                        Op_code = Flags.FileFlags.AllowOP
                    };

                    sender.SendMessage(new DataPackage
                    {
                        Client = remoteClient,
                        Data = FileInfoConvert.ConvertToBytes_Response(frr)
                    });

                    NewTeach_DAL_Server.ReceiveFile rece = new NewTeach_DAL_Server.ReceiveFile(remoteClient, new WriteFile(path), rfr.File_length);
                    if (!rece.Receive())
                    {
                        sql.DeleteFile(rfr.User_id, rfr.File_name, key);

                        frr = new FileRequestResponse_mod
                        {
                            Uid = rfr.Uid,
                            Op_code = Flags.FileFlags.FileOPFailed
                        };

                        sender.SendMessage(new DataPackage
                        {
                            Client = remoteClient,
                            Data = FileInfoConvert.ConvertToBytes_Response(frr)
                        });
                    }
                    else
                    {
                        frr = new FileRequestResponse_mod
                        {
                            Uid = rfr.Uid,
                            Op_code = Flags.FileFlags.FileOPSucceed
                        };

                        sender.SendMessage(new DataPackage
                        {
                            Client = remoteClient,
                            Data = FileInfoConvert.ConvertToBytes_Response(frr)
                        });
                    }

                }
                else
                {
                    frr = new FileRequestResponse_mod
                    {
                        Uid = rfr.Uid,
                        Op_code = Flags.FileFlags.FileExistsTrue
                    };

                    sender.SendMessage(new DataPackage
                    {
                        Client = remoteClient,
                        Data = FileInfoConvert.ConvertToBytes_Response(frr)
                    });
                }


                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
