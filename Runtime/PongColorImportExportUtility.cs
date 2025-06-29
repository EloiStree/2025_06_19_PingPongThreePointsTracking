using System;
using UnityEngine;

namespace Eloi.PongTracking
{
    public static class PongColorImportExportUtility {

        public static void ParseF(char f, out byte value)
        {
            value = 0;
            switch (f) {
                case '0': value = 0; break;
                case '1': value = 1; break;
                case '2': value = 2; break;
                case '3': value = 3; break;
                case '4': value = 4; break;
                case '5': value = 5; break;
                case '6': value = 6; break;
                case '7': value = 7; break;
                case '8': value = 8; break;
                case '9': value = 9; break;
                case 'a': case 'A': value = 10; break;
                case 'b': case 'B': value = 11; break;
                case 'c': case 'C': value = 12; break;
                case 'd': case 'D': value = 13; break;
                case 'e': case 'E': value = 14; break;
                case 'f': case 'F': value = 15; break;
            }
        }
        public static void ParseFF(char fLeft, char fRight, out byte value)
        {
            ParseF(fLeft, out byte leftByte);
            ParseF(fRight, out byte rightByte);
            throw new System.NotImplementedException();
        }

        public static void ImportFromText(string m_textToImport, out STRUCT_MinMaxColorWithFlatColorThreshold colorFilterFound) {

            colorFilterFound = new STRUCT_MinMaxColorWithFlatColorThreshold();
            m_textToImport = m_textToImport.Trim().Replace("  ", " ");
            string[] tokens = m_textToImport.Split(' ');
            colorFilterFound.ResetToDefault();
            if (tokens.Length == 3 || tokens.Length == 2)
            {
                // FFFFFF FFFFFF 100
                throw new System.NotImplementedException();

            }
            else
            {
                //
                if (tokens.Length >= 1)
                {
                    byte.TryParse(tokens[0], out colorFilterFound.m_filter.m_minColorRed);
                }
                if (tokens.Length >= 2)
                {
                    byte.TryParse(tokens[1], out colorFilterFound.m_filter.m_minColorGreen);
                }
                if (tokens.Length >= 3)
                {
                    byte.TryParse(tokens[2], out colorFilterFound.m_filter.m_minColorBlue);
                }
                if (tokens.Length >= 4)
                {
                    byte.TryParse(tokens[3], out colorFilterFound.m_filter.m_maxColorRed);
                }
                if (tokens.Length >= 5)
                {
                    byte.TryParse(tokens[4], out colorFilterFound.m_filter.m_maxColorGreen);
                }
                if (tokens.Length >= 6)
                {
                    byte.TryParse(tokens[5], out colorFilterFound.m_filter.m_maxColorBlue);
                }
                if (tokens.Length >= 7)
                {
                    byte.TryParse(tokens[6], out byte thresholdPercent);
                    colorFilterFound.m_threshold.m_flatColorDeltaPercent = thresholdPercent / 100f;
                }
            }
        }

        public static void BuildExportMinMaxColor32AsDigit(out string exportAsDigit,
            byte minRed255,
            byte minGreen255,
            byte minBlue255,
            byte maxRed255,
            byte maxGreen255,
            byte maxBlue255)
        {
            exportAsDigit = $"{minRed255} {minGreen255} {minBlue255} {maxRed255} {maxGreen255} {maxBlue255}";
        }
        public static void BuildExportMinMaxColor32AsDigit(out string exportAsDigit,
            byte minRed255,
            byte minGreen255,
            byte minBlue255,
            byte maxRed255,
            byte maxGreen255,
            byte maxBlue255,
            byte threshold100Percent)
        {
            exportAsDigit = $"{minRed255} {minGreen255} {minBlue255} {maxRed255} {maxGreen255} {maxBlue255} {threshold100Percent}";
        }

        public static void BuildExportMinMaxColor32AsDigit(out string exportAsText, Color32 minColorRange, Color32 maxColorRange)
        {
            exportAsText = $"{minColorRange.r}" +
                $" {minColorRange.g}" +
                $" {minColorRange.b}" +
                $" {maxColorRange.r}" +
                $" {maxColorRange.g}" +
                $" {maxColorRange.b}";
         }
        public static void BuildExportMinMaxColor32AsDigit(out string exportAsText, Color32 minColorRange, Color32 maxColorRange, byte threshold100Percent)
        {
            exportAsText = $"{minColorRange.r}" +
                $" {minColorRange.g}" +
                $" {minColorRange.b}" +
                $" {maxColorRange.r}" +
                $" {maxColorRange.g}" +
                $" {maxColorRange.b}" +
                $" {threshold100Percent}";
        }
    }
}
