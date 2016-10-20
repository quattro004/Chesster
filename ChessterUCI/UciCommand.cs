﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ChessterUci
{
    /// <summary>
    /// Tell engine to use the uci (universal chess interface),	this will be sent
    /// once as a first command after program boot to tell the engine to switch to uci mode.
    /// After receiving the uci command the engine must identify itself with the "id" command
    /// and send the "option" commands to tell the GUI which engine settings the engine 
    /// supports if any. After that the engine should send "uciok" to acknowledge the uci mode.
    /// If no uciok is sent within a certain time period, the engine task will be killed by 
    /// the client.
    /// </summary>
    public class UciCommand : ChessCommand
    {
        private const string UCIOK = "uciok";
        private const string ID = "id";
        private const string OPTION = "option";

        /// <summary>
        /// Initializes the "uci" command for use with the engine controller.
        /// </summary>
        /// <param name="engineController">Engine controller which manages the chess engine
        /// process.</param>
        public UciCommand(IEngineController engineController) : base(engineController)
        {
            if (engineController == null)
            {
                throw new ChessterEngineException(Messages.NullEngineController);
            }
            engineController.DataReceived += EngineController_DataReceived;
        }

        /// <summary>
        /// Command used to communicate with the chess engine ("uci").
        /// </summary>
        public override string CommandText
        {
            get
            {
                return "uci";
            }
        }

        /// <summary>
        /// Options received after initialization.
        /// </summary>
        public Dictionary<string, OptionData> Options { get; private set; } = new Dictionary<string, OptionData>();

        /// <summary>
        /// Indicates whether the engine intialized successfully.
        /// </summary>
        public bool ReceivedUciOk { get; private set; }

        /// <summary>
        /// Occurs when data is received from the engine controller after sending this command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EngineController_DataReceived(object sender, DataReceivedEventArgs e)
        {
            ChessCommandTraceSource.TraceInformation($"EngineController_DataReceived data is {e.Data}.");
            if (e.Data != null)
            {
                if (e.Data.StartsWith(ID))
                {
                    ParseIdName(e.Data);
                    ParseIdAuthor(e.Data);
                }
                else if (e.Data.StartsWith(OPTION))
                {
                    ParseOption(e.Data);
                }
                else if (e.Data.StartsWith(UCIOK))
                {
                    ReceivedUciOk = true;
                }
            }
        }

        /// <summary>
        /// Parses options that the chess engine supports. They will be added to the Options property.
        /// </summary>
        /// <param name="data"></param>
        private void ParseOption(string data)
        {
            var containsDefaultOption = data.Contains("default");
            var containsMaxOption = data.Contains("max");
            var pattern = @"name (?<name>.*) type (?<type>.*)"; // Start off with the simplest pattern.
            //
            // If the data contains max then all options are included.
            if (containsMaxOption)
            {
                pattern = @"name (?<name>.*) type (?<type>.*) default (?<default>.*) min (?<min>.*) max (?<max>.*)";
            }
            else if (containsDefaultOption)
            {
                pattern = @"name (?<name>.*) type (?<type>.*) default (?<default>.*)";
            }

            var options = Regex.Match(data, pattern);

            if(options.Groups != null)
            {
                var optionData = new OptionData();
                if (options.Groups["name"].Success)
                {
                    optionData.Name = options.Groups["name"].Value;
                }
                if (options.Groups["type"].Success)
                {
                    optionData.OptionType = options.Groups["type"].Value;
                }
                if (options.Groups["default"].Success)
                {
                    optionData.Default = options.Groups["default"].Value;
                }
                if (options.Groups["min"].Success)
                {
                    optionData.Min = options.Groups["min"].Value;
                }
                if (options.Groups["max"].Success)
                {
                    optionData.Max = options.Groups["max"].Value;
                }
                Options.Add(string.Format("{0}{1}", OPTION, Regex.Replace(optionData.Name, @"\s+", string.Empty)), optionData);
            }
        }

        /// <summary>
        /// Adds the Id option's name component.
        /// </summary>
        /// <param name="data"></param>
        private void ParseIdName(string data)
        {
            var name = new Regex("id name ");

            if (name.IsMatch(data))
            {
                var value = name.Split(data);
                if(value.Length > 1)
                {
                    var optionData = new OptionData();
                    optionData.Name = ID;
                    optionData.Id = new IdData { Name = value[1] };
                    Options.Add(ID, optionData);
                }
            }
        }

        /// <summary>
        /// Adds the Id option's author component.
        /// </summary>
        /// <param name="data"></param>
        private void ParseIdAuthor(string data)
        {
            var name = new Regex("id author ");

            if (name.IsMatch(data))
            {
                var value = name.Split(data);
                if (value.Length > 1)
                {
                    OptionData optionData;
                    if (Options.TryGetValue(ID, out optionData))
                    {
                        optionData.Id.Author = value[1];
                    }
                }
            }
        }
    }
}