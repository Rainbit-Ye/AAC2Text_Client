using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Grpc.Core;
using Manager;
using Unity.VisualScripting;
using UnityEngine;

public class GrpcClientManager : SingletonPersistentMono<GrpcClientManager>
{
    [Header("服务器配置")]
    [SerializeField] private string serverAddress = "localhost:50051";
    
    private ChannelBase _channel;
    private Service.ServiceClient _Client;

    public Service.ServiceClient Client
    {
        get{ return _Client; }
    }
    
    private void Start()
    {
        _channel = new Channel(serverAddress, ChannelCredentials.Insecure);
        _Client = new Service.ServiceClient(_channel);
        _ = ConnectedServer();

        Debug.Log("gRPC client initialized");
    }
    
    
    private async Task ConnectedServer()
    {
        ConnectCheckRequest request = new ConnectCheckRequest();
        request.Msg = serverAddress;
        
        var response = await _Client.ConnectCheckAsync(request);
        // 处理响应
        OnConnectedServer(response);
    }

    public async Task SendDispersesIcon(List<string> labels, float value)
    {
        try
        {
            // 创建请求
            var request = new AACDisperseIconSend();
            request.Csid = CSID.AacDispersesIconSend;
            request.IconLabel.AddRange(labels);

            // 发送请求并等待响应
            var response = await _Client.ProcessAACMessageAsync(request);
            
        }
        catch (RpcException e)
        {
            Debug.LogError("RPC failed: " + e.Status);
        }
    }

    private void OnConnectedServer(ConnectCheckResponse response)
    {
        if (response.Result == RESULT.Success)
        {
            Debug.Log("Connected server successfully");
            LoadingManager.Ins.StartLoad();
        }
    }

    private void OnDispersesIcon(AACDisperseIconToText response)
    {
        if (response.Csid == CSID.AacDispersesIconToText)
        {
            SystemManager.Ins.AnalysisMessage(response.Csid);
            // UI那边响应
        }
    }
    
    void OnDestroy()
    {
        // 关闭连接
        if (_channel != null)
        {
            _channel.ShutdownAsync().Wait();
        }
    }
}
