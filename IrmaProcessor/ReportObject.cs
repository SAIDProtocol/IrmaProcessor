using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace IrmaProcessor
{
	public class ReportObject
	{
		public bool finished { private set; get; }
		public TextWriter Writer { set; get; }

		private Dictionary<string, int> keys = new Dictionary<string, int>();
		private Dictionary<int, int> counts = new Dictionary<int, int>();
		private Dictionary<string, Func<string>> funcKeys = new Dictionary<string, Func<string>>();

		public void SetKey(string name, int key)
		{
			keys.Add(name, key);
			counts.Add(key, 0);
		}

		public void SetKey(string name, Func<string> func)
		{
			funcKeys[name] = func;
		}

		public int this[int key]
		{
			get { return counts[key]; }
			set { counts[key] = value; }
		}

		private Thread thread { set; get; }

		public ReportObject()
		{
			finished = false;
			thread = new Thread(ReportThread);
			Writer = Console.Out;
		}

		public void BeginReport()
		{
			if (thread == null || thread.IsAlive)
				return;
			thread.Start();
		}

		public void EndReport()
		{
			finished = true;
			thread.Join();
			thread = null;
		}

		private void ReportThread()
		{
			while (!finished)
			{
				WriteContent(Writer);
				System.Threading.Thread.Sleep(1000);
			}
			WriteContent(Writer);
			Writer.WriteLine();
		}

		public void WriteContent(TextWriter writer)
		{
			writer.Write("\r");
			foreach (var k in keys)
			{
				writer.Write("{0}={1:#,##0},", k.Key, counts[k.Value]);
			}
			foreach (var k in funcKeys)
			{
				writer.Write("{0}={1:#,##0},", k.Key, k.Value());
			}
			writer.Write("                 \r");

		}
	}}

