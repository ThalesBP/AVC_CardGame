using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public abstract class NetworkClient
{
	private string currentHost = "localhost";
	private int currentRemotePort = 0;
	
	protected Socket client = null;
	
//	public const int BUFFER_SIZE = 34;
    protected byte[] inputBuffer;// = new byte[ BUFFER_SIZE ];
    protected byte[] outputBuffer;// = new byte[ BUFFER_SIZE ]; 

	private void connectCallback( IAsyncResult ar ) 
	{
		try 
		{
			Socket handle = (Socket) ar.AsyncState;
			
			handle.EndConnect( ar );
		}
		catch( Exception e ) 
		{
			Debug.Log( e.ToString() );
		}
	}

	private void writeCallback( IAsyncResult ar ) {
		
		try 
		{
			Socket handle = (Socket) ar.AsyncState;
			
			/*int bytesSent =*/ handle.EndSend( ar );
			
			//Debug.Log( "Sent " + bytesSent.ToString() + " bytes: " + Encoding.ASCII.GetString( outputBuffer ) );
		}
		catch( Exception e ) 
		{		
			Debug.Log( e.ToString() );
		}
	}

    /// <summary>
    /// Connect the specified host, remotePort, localPort and bufferSize.
    /// </summary>
    /// <param name="host">Host.</param>
    /// <param name="remotePort">Remote port.</param>
    /// <param name="localPort">Local port.</param>
    /// <param name="bufferSize">Buffer size.</param>
    public virtual void Connect( string host, int remotePort, int localPort, int bufferSize) 
	{
        inputBuffer = new byte[ bufferSize ];
        outputBuffer = new byte[ bufferSize ]; 
      /*  for (int i = 0; i < bufferSize; i++)
        {
            inputBuffer[i] = 0;
            outputBuffer[i] = 0;
        }*/
        
		if( !client.Connected || host != currentHost || remotePort != currentRemotePort ) 
		{
			Debug.Log( "Trying to connect to host " + host + " and port " + remotePort.ToString() );
			try 
			{
				Debug.Log( "Connecting to: host: " + host + " - port: " + remotePort.ToString() );

				client.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true );
				client.ExclusiveAddressUse = false;
                client.ReceiveBufferSize = bufferSize;
                client.SendBufferSize = bufferSize;

				/*if( !client.IsBound )
				{
					IPEndPoint localIpAddress = new IPEndPoint( IPAddress.Any, localPort );
					client.Bind( (EndPoint) localIpAddress );
				}*/

				IPAddress ipRemoteHost = Dns.GetHostEntry( host ).AddressList[ 0 ];
				Debug.Log( "Ip Remote Host: " + ipRemoteHost.ToString() );
				IPEndPoint remoteIpAddress = new IPEndPoint( ipRemoteHost, remotePort );
				client.Connect( (EndPoint) remoteIpAddress );
				Debug.Log( "Bound to: " + client.LocalEndPoint.ToString() );
				Debug.Log( "Connected to: " + client.RemoteEndPoint.ToString() );
				currentHost = host;
				currentRemotePort = remotePort;
			} 
			catch( Exception e ) 
			{
				Disconnect();
				Debug.Log("Error Connecting: " + e.ToString() );
			}
		}
	}

	public abstract string ReceiveString();

	public abstract string[] QueryData( string key );

	public void SendString( string message ) 
	{
		if( client.Connected ) 
		{
            try 
			{
				Array.Clear( outputBuffer, 0, outputBuffer.Length );
				Encoding.ASCII.GetBytes( message, 0, message.Length, outputBuffer, 0 );
                
				client.BeginSend( outputBuffer, 0, outputBuffer.Length, 
				                 SocketFlags.None, new AsyncCallback( writeCallback ), client );
                
			}
			catch( Exception e )
            {
                Debug.Log("Error Sending: " + e.ToString());
            }
		}
	}

	public void SendByte( byte[] message ) 
	{
		if( client.Connected ) 
		{
			try 
			{
				client.BeginSend( message, 0, message.Length, 
				                 SocketFlags.None, new AsyncCallback( writeCallback ), client );
			}
			catch( Exception e ) 
			{
				Debug.Log("Error Sending: " + e.ToString() );
			}	
		}
	}

	public virtual void Disconnect()
	{
		try
		{
			client.Close();
		}
		catch( Exception e )
		{
			Debug.Log("Error Desconnecting: " + e.ToString() );
		}

		currentHost = "localhost";
		currentRemotePort = 0;

		Debug.Log( "Encerrando conexao" );
	}

	~NetworkClient() 
	{
		Disconnect();
	}
}
