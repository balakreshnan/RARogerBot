﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace rocksecbot.Bots.Model
{
    /// <summary>
    /// Array of translated results from Translator API v3.
    /// </summary>
    internal class TranslatorResponse
    {
        [JsonProperty("translations")]
        public IEnumerable<TranslatorResult> Translations { get; set; }
    }
}
