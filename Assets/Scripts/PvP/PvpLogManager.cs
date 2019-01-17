using UnityEngine;
using System;
using System.IO;
using System.Collections;

public class PvpLogManager
{
	public static void Log(int tableID, string log)
	{
		#if UNITY_STANDALONE_WIN
			string logPath = Application.dataPath + "\\Log\\";
			if(!Directory.Exists(logPath))
			{
				Directory.CreateDirectory(logPath);
			}

			string filePath = logPath + tableID + ".txt";
			if(!File.Exists(filePath))
			{
				File.Create(filePath).Close();
			}

			using(StreamWriter streamWriter = new StreamWriter(filePath, true, System.Text.Encoding.UTF8))
			{
				streamWriter.WriteLine(log);
				streamWriter.Flush();
			}
		#endif
	}
}
