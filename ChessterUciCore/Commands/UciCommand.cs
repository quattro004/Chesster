﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChessterUciCore.Commands
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
        public UciCommand() : base()
        {
        }

        /// <summary>
        /// Reference to the chess engine controller which manages the actual process.
        /// </summary>
        internal override IEngineController ChessEngineController
        {
            get
            {
                return base.ChessEngineController;
            }

            set
            {
                base.ChessEngineController = value;
                if (base.ChessEngineController != null)
                {
                    base.ChessEngineController.DataReceived += EngineController_DataReceived;
                }
            }
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
        public Dictionary<string, OptionData> Options { get; } = new Dictionary<string, OptionData>();

        /// <summary>
        /// This is needed for copyprotected engines. After the uciok command the engine can tell the GUI, 
        /// that it will check the copy protection now. This is done by "copyprotection checking". If the check 
        /// is ok the engine should send "copyprotection ok", otherwise "copyprotection error". If there is an
        /// error the engine should not function properly but should not quit alone. If the engine reports 
        /// "copyprotection error" the GUI should not use this engine and display an error message instead!
        /// </summary>
        public string CopyProtectionInfo { get; private set; }

        /// <summary>
        /// This is needed for engines that need a username and/or a code to function with all features.
        /// Analog to the "copyprotection" command the engine can send "registration checking"
        /// after the uciok command followed by either "registration ok" or "registration error".
        /// Also after every attempt to register the engine it should answer with "registration checking"
        /// and then either "registration ok" or "registration error".
        /// In contrast to the "copyprotection" command, the GUI can use the engine after the engine has
        /// reported an error, but should inform the user that the engine is not properly registered
        /// and might not use all its features. In addition the GUI should offer to open a dialog to
        /// enable registration of the engine. To try to register an engine the GUI can send the "register" 
        /// command. The GUI has to always answer with the "register" command if the engine sends 
        /// "registration error" at engine startup (this can also be done with "register later")
        /// and tell the user somehow that the engine is not registered. This way the engine knows that 
        /// the GUI can deal with the registration procedure and the user will be informed that the engine 
        /// is not properly registered.
        /// </summary>
        public RegistrationStatus RegistrationInfo { get; private set; }

        /// <summary>
        /// Performs disposal for this command.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ChessEngineController != null)
                {
                    ChessEngineController.DataReceived -= EngineController_DataReceived;
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Occurs when data is received from the engine controller after sending this command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EngineController_DataReceived(object sender, ChessCommandReceivedEventArgs e)
        {
            var data = e.Data;

            if (ResponseIsNotNullOrEmpty(data))
            {
                if (data.StartsWith(ID))
                {
                    ParseIdName(data);
                    ParseIdAuthor(data);
                }
                else if (data.StartsWith(OPTION))
                {
                    ParseOption(data);
                }
                else if (data.StartsWith(UCIOK))
                {
                    CommandResponseReceived = true;
                }
                else if (data.StartsWith("copyprotection"))
                {
                    CopyProtectionInfo = data;
                }
                else if (data.StartsWith("registration"))
                {
                    RegistrationInfo = GetRegistrationStatus(data);
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
