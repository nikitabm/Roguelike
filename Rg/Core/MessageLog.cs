﻿using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rg.Core
{
    // Represents a queue of messages that can be added to
    // Has a method for and drawing to an RLConsole
    public class MessageLog
    {
        // Define the maximum number of lines to store
        private int _maxLines;

        // Use a Queue to keep track of the lines of text
        // The first line added to the log will also be the first removed
        private readonly Queue<string> _lines;

        public MessageLog(int maxLines = 9)
        {
            _maxLines = maxLines;
            _lines = new Queue<string>();
        }
        public void Clear()
        {
            _lines.Clear();
        }

        // Add a line to the MessageLog queue
        public void Add(string message)
        {
            _lines.Enqueue(message);

            // When exceeding the maximum number of lines remove the oldest one.
            if (_lines.Count > _maxLines)
            {
                _lines.Dequeue();
            }
        }

        // Draw each line of the MessageLog queue to the console
        public void Draw(RLConsole console)
        {
            string[] lines = _lines.ToArray();
            for (int i = 0; i < lines.Length; i++)
            {
                console.Print(1, i + 1, lines[i], RLColor.White);
            }
        }
    }
}
