using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Microsoft.CognitiveServices.SpeechRecognition
{

    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Runtime.CompilerServices;
    using System.Windows;
    public class SppechMain
    {
        /// <summary>
        /// You can also put the primary key in app.config, instead of using UI.
        /// string subscriptionKey = ConfigurationManager.AppSettings["primaryKey"];
        /// </summary>
        private string subscriptionKey;

        /// <summary>
        /// The data recognition client
        /// </summary>
       // private DataRecognitionClient dataClient;

        /// <summary>
        /// The microphone client
        /// </summary>
       // private MicrophoneRecognitionClient micClient;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is microphone client with intent.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is microphone client with intent; otherwise, <c>false</c>.
        /// </value>
        public bool IsMicrophoneClientWithIntent { get; set; }

    }
}