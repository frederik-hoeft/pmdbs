﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pmdbs
{
    public static class Password
    {
        public sealed class SimplifiedResult
        {
            public readonly string Id;
            public readonly int Score;
            public readonly string Host;
            private SimplifiedResult(string id, int score, string host)
            {
                Id = id;
                Score = score;
                Host = host;
            }

            public static SimplifiedResult Create(string id, int score, string host)
            {
                return new SimplifiedResult(id, score, host);
            }

            public Result ToResult()
            {
                return Result.FromScore(Score);
            }
        }
        public sealed class Result
        {
            private readonly string _complexity;
            private readonly string _grade;
            private readonly int _score;
            private readonly int _isCompromised = -1;
            private readonly int _timesSeen = 0;
            private readonly System.Drawing.Bitmap _icon;

            public Result(string complexity, string grade, int score)
            {
                _complexity = complexity;
                _grade = grade;
                _score = score;
                _icon = GetIcon(_grade);
            }
            public Result(string complexity, string grade, int score, int isCompromised, int timesSeen)
            {
                _complexity = complexity;
                _grade = grade;
                _score = score;
                _isCompromised = isCompromised;
                _timesSeen = timesSeen;
                _icon = GetIcon(_grade);
            }

            public Result(Result result, int isCompromised, int timesSeen)
            {
                _complexity = result.Complexity;
                _grade = result.Grade;
                _score = result.Score;
                _isCompromised = isCompromised;
                _timesSeen = timesSeen;
                _icon = result.Icon;
            }

            public System.Drawing.Bitmap Icon
            {
                get { return _icon; }
            }

            public string Complexity
            {
                get { return _complexity; }
            }

            public string Grade
            {
                get { return _grade; }
            }

            public int Score
            {
                get { return _score; }
            }

            public int IsCompromised
            {
                get { return _isCompromised; }
            }

            public int TimesSeen
            {
                get { return _timesSeen; }
            }

            private System.Drawing.Bitmap GetIcon(string grade)
            {
                switch (grade)
                {
                    case "A": return Properties.Resources.a;
                    case "A-": return Properties.Resources.am;
                    case "B+": return Properties.Resources.bp;
                    case "B": return Properties.Resources.b;
                    case "B-": return Properties.Resources.bm;
                    case "C+": return Properties.Resources.cp;
                    case "C": return Properties.Resources.c;
                    case "C-": return Properties.Resources.cm;
                    case "D+": return Properties.Resources.dp;
                    case "D": return Properties.Resources.d;
                    case "D-": return Properties.Resources.dm;
                    case "F": return Properties.Resources.f;
                    default: return null;
                }
            }

            public static Result FromScore(int score)
            {
                /* Determine complexity and grade based on overall score */
                string passwordComplexity = "Too Short";
                string grade = "F";
                if (score < 35)
                {
                    grade = "F";
                    passwordComplexity = "Embarrassing";
                }
                else if (score >= 35 && score < 55)
                {
                    grade = "D-";
                    passwordComplexity = "Very Weak";
                }
                else if (score >= 55 && score < 75)
                {
                    grade = "D";
                    passwordComplexity = "Very Weak";
                }
                else if (score >= 75 && score < 95)
                {
                    grade = "D+";
                    passwordComplexity = "Weak";
                }
                else if (score >= 95 && score < 110)
                {
                    grade = "C-";
                    passwordComplexity = "Weak";
                }
                else if (score >= 110 && score < 125)
                {
                    grade = "C";
                    passwordComplexity = "Okay";
                }
                else if (score >= 125 && score < 140)
                {
                    grade = "C+";
                    passwordComplexity = "Okay";
                }
                else if (score >= 140 && score < 155)
                {
                    grade = "B-";
                    passwordComplexity = "Good";
                }
                else if (score >= 155 && score < 170)
                {
                    grade = "B";
                    passwordComplexity = "Good";
                }
                else if (score >= 170 && score < 185)
                {
                    grade = "B+";
                    passwordComplexity = "Strong";
                }
                else if (score >= 185 && score < 200)
                {
                    grade = "A-";
                    passwordComplexity = "Strong";
                }
                else if (score >= 200)
                {
                    grade = "A";
                    passwordComplexity = "Very Strong";
                }

                return new Result(passwordComplexity, grade, score);
            }
        }
        public struct Results
        {
            public const int TooShort = -1;
            public const int Embarrassing = 34;
            public const int VeryWeak = 74;
            public const int Weak = 109;
            public const int Okay = 139;
            public const int Good = 169;
            public const int Strong = 199;
            public const int VeryStrong = 200;
        }
        public static class Security
        {
            public static Task<Result> OnlineCheckAsync(string password)
            {
                return Task.Run(() => OnlineCheck(password));
            }

            public static Task<Result> CheckAsync(string password)
            {
                return Task.Run(() => Check(password));
            }

            public static Result OnlineCheck(string password)
            {
                Result result = new Result(null, null, -1);
                int isCompromized = -1;
                int timesSeen = 0;
                try
                {
                    int[] onlineResult = IsCompromised(password);
                    isCompromized = onlineResult[0];
                    timesSeen = onlineResult[1];
                }
                catch { }
                return new Result(result, isCompromized, timesSeen);
            }
            public static Result SimpleCheck(string password)
            {
                return Analyze(password);
            }
            public static Result Check(string password)
            {
                Result offlineResult = Analyze(password);
                int isCompromized = -1;
                int timesSeen = 0;
                try
                {
                    int[] onlineResult = IsCompromised(password);
                    isCompromized = onlineResult[0];
                    timesSeen = onlineResult[1];
                }
                catch { }
                return new Result(offlineResult, isCompromized, timesSeen);
            }
            private static int[] IsCompromised(string password)
            {
                string result = CryptoHelper.SHA1Hash(password).ToUpper();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // get a list of all the possible passwords where the first 5 digits of the hash are the same
                string url = "https://api.pwnedpasswords.com/range/" + result.Substring(0, 5);
                int isCompromised = -1;
                int timesSeen = 0;
                WebRequest request = WebRequest.Create(url);
                using (Stream response = request.GetResponse().GetResponseStream())
                using (StreamReader reader = new StreamReader(response))
                {
                    // look at each possibility and compare the rest of the hash to see if there is a match
                    string hashToCheck = result.Substring(5);
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null)
                        {
                            isCompromised = 0;
                            break;
                        }
                        string[] parts = line.Split(':');
                        if (parts[0].Equals(hashToCheck))
                        {
                            isCompromised = 1;
                            timesSeen = Convert.ToInt32(parts[1]);
                            break;
                        }
                    }
                }
                return new int[] { isCompromised, timesSeen };
            }
            private static Result Analyze(string password)
            {
                // Originally created by: Jeff Todnem (http://www.todnem.com/) in Javascript
                // Ported to C# by Frederik Höft
                int score = 0, upperCaseCount = 0, lowerCaseCount = 0, numberCount = 0, symbolCount = 0, unicodeCount = 0, middleSpecialCharacterCount = 0, nRequirements = 0, uniqueCharacterCount = 0, repeatedCharacterCount = 0, repeatedCharacterDeduction = 0, consecutiveUpperCaseCount = 0, consecutiveLowerCaseCount = 0, consecutiveNumberCount = 0, sequentialLetterCount = 0, sequentialNumberCount = 0, sequentialSymbolCount = 0, sequentialUnicodeCount = 0, sequentialCharacterCount = 0, fulfilledRequirementsCount = 0;
                int middleSpecialCharacterMultiplier = 2, consecutiveUpperCaseMultiplier = 2, consecutiveLowerCaseMultiplier = 2, consecutiveNumberMultiplier = 2, unicodeMultiplier = 30;
                int sequentialLetterMultiplier = 3, sequentialNumberMultiplier = 3, sequentialSymbolMultiplier = 2, sequentialUnicodeMultiplier = 1;
                int lengthMultipier = 6, numberMultiplier = 4;
                int symbolMultiplier = 8;
                int upperCaseLastIndex = -1, lowerCaseLastIndex = -1, numberLastIndex = -1;
                string alphabet = "abcdefghijklmnopqrstuvwxyz";
                string keyboard = "qwertyuiopasdfghjklzxcvbnm";
                string numbers = "01234567890";
                string symbols = " !\"#$%&\'()*+,-./:;<=>?@[\\]^_{|}~";
                int minimumPasswordLength = 12;
                score = password.Length * lengthMultipier;
                int passwordLength = password.Length;

                /* Loop through password to check for Symbol, Numeric, Lowercase and Uppercase pattern matches */
                for (int a = 0; a < passwordLength; a++)
                {
                    if (Regex.Match(password[a].ToString(), @"[A-Z]").Success)
                    {
                        // Check if previous character was upper case as well
                        if (upperCaseLastIndex != 0 && (upperCaseLastIndex + 1) == a)
                        {
                            consecutiveUpperCaseCount++;
                        }
                        upperCaseLastIndex = a;
                        upperCaseCount++;
                    }
                    else if (Regex.Match(password[a].ToString(), @"[a-z]").Success)
                    {
                        // Check if previous character was lower case as well
                        if (lowerCaseLastIndex != 0 && (lowerCaseLastIndex + 1) == a)
                        {
                            consecutiveLowerCaseCount++;
                        }
                        lowerCaseLastIndex = a;
                        lowerCaseCount++;
                    }
                    else if (Regex.Match(password[a].ToString(), @"[0-9]").Success)
                    {
                        if (a > 0 && a < (passwordLength - 1))
                        {
                            middleSpecialCharacterCount++;
                        }
                        // Check if previous character was a number as well
                        if (numberLastIndex != 0 && (numberLastIndex + 1) == a)
                        {
                            consecutiveNumberCount++;
                        }
                        numberLastIndex = a;
                        numberCount++;
                    }
                    else if (symbols.Contains(password[a].ToString()))
                    {
                        if (a > 0 && a < (passwordLength - 1))
                        {
                            middleSpecialCharacterCount++;
                        }
                        symbolCount++;
                    }
                    else
                    {
                        if (a > 0 && a < (passwordLength - 1))
                        {
                            middleSpecialCharacterCount++;
                        }
                        unicodeCount++;
                    }
                    /* Internal loop through password to check for repeat characters */
                    bool repeatedCharactersExist = false;
                    for (int b = 0; b < passwordLength; b++)
                    {
                        if (password[a] == password[b] && a != b)
                        { /* repeat character exists */
                            repeatedCharactersExist = true;
                            /* 
                            Calculate increment deduction based on proximity to identical characters
                            Deduction is incremented each time a new match is discovered
                            Deduction amount is based on total password length divided by the
                            difference of distance between currently selected match
                            */
                            if (symbols.Contains(password[a]))
                            {
                                repeatedCharacterDeduction += symbolMultiplier;
                            }
                            else if (!Regex.Match(password[a].ToString(), @"[A-Za-z0-9]").Success)
                            {
                                repeatedCharacterDeduction += unicodeMultiplier;
                            }
                            repeatedCharacterDeduction += Math.Abs(passwordLength / (b - a));
                        }
                    }
                    if (repeatedCharactersExist)
                    {
                        repeatedCharacterCount++;
                        uniqueCharacterCount = passwordLength - repeatedCharacterCount;
                        repeatedCharacterDeduction = (uniqueCharacterCount != 0) ? Convert.ToInt32(Math.Ceiling((double)repeatedCharacterDeduction / uniqueCharacterCount)) : Convert.ToInt32(Math.Ceiling((double)repeatedCharacterDeduction));
                    }
                }

                /* Check for sequential alpha string patterns (forward and reverse) */
                for (int s = 0; s < 23; s++)
                {
                    string sequence = alphabet.Substring(s, 3);
                    string reversedSequence = HelperMethods.ReverseString(sequence);
                    string keyboardSequence = keyboard.Substring(s, 3);
                    string reversedKeyboardSequence = HelperMethods.ReverseString(keyboardSequence);
                    if (password.ToLower().Contains(sequence.ToLower()) || password.ToLower().Contains(reversedSequence.ToLower()) || password.ToLower().Contains(keyboardSequence) || password.ToLower().Contains(reversedKeyboardSequence))
                    {
                        sequentialLetterCount++;
                        sequentialCharacterCount++;
                    }
                }

                /* Check for sequential numeric string patterns (forward and reverse) */
                for (int s = 0; s < numbers.Length - 2; s++)
                {
                    string sequence = numbers.Substring(s, 3);
                    string reversedSequence = HelperMethods.ReverseString(sequence);
                    if (password.Contains(sequence) || password.Contains(reversedSequence))
                    {
                        sequentialNumberCount++;
                        sequentialCharacterCount++;
                    }
                }

                /* Check for sequential symbol string patterns (forward and reverse) */
                for (int s = 0; s < symbols.Length - 2; s++)
                {
                    string sequence = symbols.Substring(s, 3);
                    string reversedSequence = HelperMethods.ReverseString(sequence);
                    if (password.Contains(sequence) || password.Contains(reversedSequence))
                    {
                        sequentialSymbolCount++;
                        sequentialCharacterCount++;
                    }
                }

                /* Modify overall score value based on usage vs requirements */

                if (upperCaseCount > 0 && upperCaseCount < passwordLength)
                {
                    score = score + ((passwordLength - upperCaseCount) * 2);
                }
                if (lowerCaseCount > 0 && lowerCaseCount < passwordLength)
                {
                    score = score + ((passwordLength - lowerCaseCount) * 2);
                }
                if (numberCount > 0 && numberCount < passwordLength)
                {
                    score = score + (numberCount * numberMultiplier);
                }
                if (symbolCount > 0)
                {
                    score = score + (symbolCount * symbolMultiplier);
                }
                if (middleSpecialCharacterCount > 0)
                {
                    score = score + (middleSpecialCharacterCount * middleSpecialCharacterMultiplier);
                }
                if (unicodeCount > 0)
                {
                    score = score + (unicodeCount * unicodeMultiplier);
                }

                /* Point deductions for poor practices */
                if ((lowerCaseCount > 0 || upperCaseCount > 0) && symbolCount == 0 && numberCount == 0)
                {  // Only Letters
                    score = score - passwordLength;
                }
                if (lowerCaseCount == 0 && upperCaseCount == 0 && symbolCount == 0 && numberCount > 0)
                {  // Only Numbers
                    score = score - passwordLength;
                }
                if (repeatedCharacterCount > 0)
                {  // Same character exists more than once
                    score = score - repeatedCharacterDeduction;
                }
                if (consecutiveUpperCaseCount > 0)
                {  // Consecutive Uppercase Letters exist
                    score = score - (consecutiveUpperCaseCount * consecutiveUpperCaseMultiplier);
                }
                if (consecutiveLowerCaseCount > 0)
                {  // Consecutive Lowercase Letters exist
                    score = score - (consecutiveLowerCaseCount * consecutiveLowerCaseMultiplier);
                }
                if (consecutiveNumberCount > 0)
                {  // Consecutive Numbers exist
                    score = score - (consecutiveNumberCount * consecutiveNumberMultiplier);
                }
                if (sequentialLetterCount > 0)
                {  // Sequential alpha strings exist (3 characters or more)
                    score = score - (sequentialLetterCount * sequentialLetterMultiplier);
                }
                if (sequentialNumberCount > 0)
                {  // Sequential numeric strings exist (3 characters or more)
                    score = score - (sequentialNumberCount * sequentialNumberMultiplier);
                }
                if (sequentialSymbolCount > 0)
                {  // Sequential symbol strings exist (3 characters or more)
                    score = score - (sequentialSymbolCount * sequentialSymbolMultiplier);
                }
                if (sequentialUnicodeCount > 0)
                {  // Sequential unicode strings exist (3 characters or more)
                    score = score - (sequentialUnicodeCount * sequentialUnicodeMultiplier);
                }
                if (sequentialCharacterCount > 0)
                {  // Sequential characters exist (3 characters or more)
                    score = score - sequentialCharacterCount;
                }

                /* Determine if mandatory requirements have been met and set image indicators accordingly */
                int[] characterCounts = new int[] { passwordLength, upperCaseCount, lowerCaseCount, numberCount, symbolCount };
                for (int c = 0; c < characterCounts.Length; c++)
                {
                    int minimumValue;
                    if (c == 0)
                    {
                        minimumValue = minimumPasswordLength;
                    }
                    else
                    {
                        minimumValue = 0;
                    }
                    if (characterCounts[c] > minimumValue)
                    {
                        fulfilledRequirementsCount++;
                    }
                }
                nRequirements = fulfilledRequirementsCount;
                int nMinReqChars;
                if (password.Length >= minimumPasswordLength) { nMinReqChars = 3; } else { nMinReqChars = 4; }
                if (nRequirements > nMinReqChars)
                {  // One or more required characters exist
                    score = score + (nRequirements * 2);
                }

                return Result.FromScore(score);
            }
        }
    }
}
