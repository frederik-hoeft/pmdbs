using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pmdbs
{
    // Originally created by: Jeff Todnem (http://www.todnem.com/) in Javascript
    // Ported to C# by Frederik Höft

    public class PasswordChecker
    {
        private static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
        public static List<object> chkPass(string pwd)
        {
            int nScore = 0, nLength = 0, nAlphaUC = 0, nAlphaLC = 0, nNumber = 0, nSymbol = 0, nMidChar = 0, nRequirements = 0, nAlphasOnly = 0, nNumbersOnly = 0, nUnqChar = 0, nRepChar = 0, nRepInc = 0, nConsecAlphaUC = 0, nConsecAlphaLC = 0, nConsecNumber = 0, nConsecSymbol = 0, nConsecCharType = 0, nSeqAlpha = 0, nSeqNumber = 0, nSeqSymbol = 0, nSeqChar = 0, nReqChar = 0;
            int nMultMidChar = 2, nMultConsecAlphaUC = 2, nMultConsecAlphaLC = 2, nMultConsecNumber = 2;
            int nMultSeqAlpha = 3, nMultSeqNumber = 3, nMultSeqSymbol = 3;
            int nMultLength = 4, nMultNumber = 4;
            int nMultSymbol = 6;
            string nTmpAlphaUC = "", nTmpAlphaLC = "", nTmpNumber = "", nTmpSymbol = "";
            string sAlphaUC = "0", sAlphaLC = "0", sNumber = "0", sSymbol = "0", sMidChar = "0", sRequirements = "0", sAlphasOnly = "0", sNumbersOnly = "0", sRepChar = "0", sConsecAlphaUC = "0", sConsecAlphaLC = "0", sConsecNumber = "0", sSeqAlpha = "0", sSeqNumber = "0", sSeqSymbol = "0";
            string sAlphas = "abcdefghijklmnopqrstuvwxyz";
            string sNumerics = "01234567890";
            string sSymbols = ")!@#$%^&*()";
            string sComplexity = "Too Short";
            int nMinPwdLen = 8;
            nScore = pwd.Length * nMultLength;
            nLength = pwd.Length;
            string[] arrPwd = Regex.Split(Regex.Replace(pwd, @"\s+", ""), @"\s*");
            int arrPwdLen = arrPwd.Length;

            /* Loop through password to check for Symbol, Numeric, Lowercase and Uppercase pattern matches */
            for (int a = 0; a < arrPwdLen; a++)
            {
                if (Regex.Match(arrPwd[a], @"[A-Z]").Success)
                {
                    if (nTmpAlphaUC != "") { if ((Convert.ToInt32(nTmpAlphaUC) + 1) == a) { nConsecAlphaUC++; nConsecCharType++; } }
                    nTmpAlphaUC = a.ToString();
                    nAlphaUC++;
                }
                else if (Regex.Match(arrPwd[a], @"[a-z]").Success)
                {
                    if (nTmpAlphaLC != "") { if ((Convert.ToInt32(nTmpAlphaLC) + 1) == a) { nConsecAlphaLC++; nConsecCharType++; } }
                    nTmpAlphaLC = a.ToString();
                    nAlphaLC++;
                }
                else if (Regex.Match(arrPwd[a], @"[0-9]").Success)
                {
                    if (a > 0 && a < (arrPwdLen - 1)) { nMidChar++; }
                    if (nTmpNumber != "") { if ((Convert.ToInt32(nTmpNumber) + 1) == a) { nConsecNumber++; nConsecCharType++; } }
                    nTmpNumber = a.ToString();
                    nNumber++;
                }
                else if (Regex.Match(arrPwd[a], @"[^ a - zA - Z0 - 9_]").Success)
                {
                    if (a > 0 && a < (arrPwdLen - 1)) { nMidChar++; }
                    if (nTmpSymbol != "") { if ((Convert.ToInt32(nTmpSymbol) + 1) == a) { nConsecSymbol++; nConsecCharType++; } }
                    nTmpSymbol = a.ToString();
                    nSymbol++;
                }
                /* Internal loop through password to check for repeat characters */
                bool bCharExists = false;
                for (int b = 0; b < arrPwdLen; b++)
                {
                    if (arrPwd[a] == arrPwd[b] && a != b)
                    { /* repeat character exists */
                        bCharExists = true;
                        /* 
                        Calculate increment deduction based on proximity to identical characters
                        Deduction is incremented each time a new match is discovered
                        Deduction amount is based on total password length divided by the
                        difference of distance between currently selected match
                        */
                        nRepInc += Math.Abs(arrPwdLen / (b - a));
                    }
                }
                if (bCharExists)
                {
                    nRepChar++;
                    nUnqChar = arrPwdLen - nRepChar;
                    nRepInc = (nUnqChar != 0) ? Convert.ToInt32(Math.Ceiling((double)nRepInc / nUnqChar)) : Convert.ToInt32(Math.Ceiling((double)nRepInc));
                }
            }

            /* Check for sequential alpha string patterns (forward and reverse) */
            for (int s = 0; s < 23; s++)
            {
                string sFwd = sAlphas.Substring(s, 3);
                string sRev = ReverseString(sFwd);
                if (pwd.ToLower().Contains(sFwd.ToLower()) || pwd.ToLower().Contains(sRev.ToLower())) { nSeqAlpha++; nSeqChar++; }
            }
             
            /* Check for sequential numeric string patterns (forward and reverse) */
            for (int s = 0; s < 8; s++)
            {
                string sFwd = sNumerics.Substring(s, 3);
                string sRev = ReverseString(sFwd);
                if (pwd.ToLower().Contains(sFwd.ToLower()) || pwd.ToLower().Contains(sRev.ToLower())) { nSeqNumber++; nSeqChar++; }
            }

            /* Check for sequential symbol string patterns (forward and reverse) */
            for (int s = 0; s < 8; s++)
            {
                string sFwd = sSymbols.Substring(s, 3);
                string sRev = ReverseString(sFwd);
                if (pwd.ToLower().Contains(sFwd.ToLower()) || pwd.ToLower().Contains(sRev.ToLower())) { nSeqSymbol++; nSeqChar++; }
            }
		
	/* Modify overall score value based on usage vs requirements */

            if (nAlphaUC > 0 && nAlphaUC < nLength)
            {
                nScore = nScore + ((nLength - nAlphaUC) * 2);
                sAlphaUC = "+ " + (nLength - nAlphaUC) * 2;
            }
            if (nAlphaLC > 0 && nAlphaLC < nLength)
            {
                nScore = nScore + ((nLength - nAlphaLC) * 2);
                sAlphaLC = "+ " + (nLength - nAlphaLC) * 2;
            }
            if (nNumber > 0 && nNumber < nLength)
            {
                nScore = nScore + (nNumber * nMultNumber);
                sNumber = "+ " + nNumber * nMultNumber;
            }
            if (nSymbol > 0)
            {
                nScore = nScore + (nSymbol * nMultSymbol);
                sSymbol = "+ " + nSymbol * nMultSymbol;
            }
            if (nMidChar > 0)
            {
                nScore = nScore + (nMidChar * nMultMidChar);
                sMidChar = "+ " + nMidChar * nMultMidChar;
            }

            /* Point deductions for poor practices */
            if ((nAlphaLC > 0 || nAlphaUC > 0) && nSymbol == 0 && nNumber == 0)
            {  // Only Letters
                nScore = nScore - nLength;
                nAlphasOnly = nLength;
                sAlphasOnly = "- " + nLength;
            }
            if (nAlphaLC == 0 && nAlphaUC == 0 && nSymbol == 0 && nNumber > 0)
            {  // Only Numbers
                nScore = nScore - nLength;
                nNumbersOnly = nLength;
                sNumbersOnly = "- " + nLength;
            }
            if (nRepChar > 0)
            {  // Same character exists more than once
                nScore = nScore - nRepInc;
                sRepChar = "- " + nRepInc;
            }
            if (nConsecAlphaUC > 0)
            {  // Consecutive Uppercase Letters exist
                nScore = nScore - (nConsecAlphaUC * nMultConsecAlphaUC);
                sConsecAlphaUC = "- " + nConsecAlphaUC * nMultConsecAlphaUC;
            }
            if (nConsecAlphaLC > 0)
            {  // Consecutive Lowercase Letters exist
                nScore = nScore - (nConsecAlphaLC * nMultConsecAlphaLC);
                sConsecAlphaLC = "- " + nConsecAlphaLC * nMultConsecAlphaLC;
            }
            if (nConsecNumber > 0)
            {  // Consecutive Numbers exist
                nScore = nScore - (nConsecNumber * nMultConsecNumber);
                sConsecNumber = "- " + nConsecNumber * nMultConsecNumber;
            }
            if (nSeqAlpha > 0)
            {  // Sequential alpha strings exist (3 characters or more)
                nScore = nScore - (nSeqAlpha * nMultSeqAlpha);
                sSeqAlpha = "- " + nSeqAlpha * nMultSeqAlpha;
            }
            if (nSeqNumber > 0)
            {  // Sequential numeric strings exist (3 characters or more)
                nScore = nScore - (nSeqNumber * nMultSeqNumber);
                sSeqNumber = "- " + nSeqNumber * nMultSeqNumber;
            }
            if (nSeqSymbol > 0)
            {  // Sequential symbol strings exist (3 characters or more)
                nScore = nScore - (nSeqSymbol * nMultSeqSymbol);
                sSeqSymbol = "- " + nSeqSymbol * nMultSeqSymbol;
            }

            /* Determine if mandatory requirements have been met and set image indicators accordingly */
            int[] arrChars = new int[] { nLength, nAlphaUC, nAlphaLC, nNumber, nSymbol };
            string[] arrCharsIds = new string[] { "nLength", "nAlphaUC", "nAlphaLC", "nNumber", "nSymbol" };
            int arrCharsLen = arrChars.Length;
            for (int c = 0; c < arrCharsLen; c++)
            {
                int minVal;
                if (arrCharsIds[c] == "nLength") { minVal = nMinPwdLen - 1; } else { minVal = 0; }
                if (arrChars[c] == minVal + 1) { nReqChar++; }
                else if (arrChars[c] > minVal + 1) { nReqChar++; }
            }
            nRequirements = nReqChar;
            int nMinReqChars;
            if (pwd.Length >= nMinPwdLen) {  nMinReqChars = 3; } else {  nMinReqChars = 4; }
            if (nRequirements > nMinReqChars)
            {  // One or more required characters exist
                nScore = nScore + (nRequirements * 2);
                sRequirements = "+ " + nRequirements * 2;
            }

            /* Determine if suggested requirements have been met and set image indicators accordingly */
            arrChars = new int[] { nAlphasOnly, nNumbersOnly, nRepChar, nConsecAlphaUC, nConsecAlphaLC, nConsecNumber, nSeqAlpha, nSeqNumber, nSeqSymbol };
            arrCharsIds = new string[] { "nAlphasOnly", "nNumbersOnly", "nRepChar", "nConsecAlphaUC", "nConsecAlphaLC", "nConsecNumber", "nSeqAlpha", "nSeqNumber", "nSeqSymbol" };
            arrCharsLen = arrChars.Length;

            /* Determine complexity based on overall score */
            if (nScore > 100) { nScore = 100; } else if (nScore < 0) { nScore = 0; }
            if (nScore >= 0 && nScore < 20) { sComplexity = "Very Weak"; }
            else if (nScore >= 20 && nScore < 40) { sComplexity = "Weak"; }
            else if (nScore >= 40 && nScore < 60) { sComplexity = "Good"; }
            else if (nScore >= 60 && nScore < 80) { sComplexity = "Strong"; }
            else if (nScore >= 80 && nScore <= 100) { sComplexity = "Very Strong"; }

            return new List<object>() { nScore, sComplexity };
        }
    }
}
