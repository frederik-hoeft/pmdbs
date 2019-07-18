package pmdbs;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;
import javax.net.ssl.HttpsURLConnection;

public class Password
{
	// --------------SINGLETON DESIGN PATTERN START
	private static Password password = null;
	
	private Password() {}
	
	public static Password GetInstance() 
	{
		if(password == null) 
		{
			password = new Password();
		}
	    return password;
	}
	// --------------SINGLETON DESIGN PATTERN END
	
    public class Result
    {
        private final String _complexity;
        private final String _grade;
        private final int _score;
        private final int _isCompromised;
        private final int _timesSeen;

        public Result(String complexity, String grade, int score)
        {
            _complexity = complexity;
            _grade = grade;
            _score = score;
            _isCompromised = -1;
            _timesSeen = 0;
        }
        public Result(String complexity, String grade, int score, int isCompromised, int timesSeen)
        {
            _complexity = complexity;
            _grade = grade;
            _score = score;
            _isCompromised = isCompromised;
            _timesSeen = timesSeen;
        }

        public Result(Result result, int isCompromised, int timesSeen)
        {
            _complexity = result.Complexity();
            _grade = result.Grade();
            _score = result.Score();
            _isCompromised = isCompromised;
            _timesSeen = timesSeen;
        }

        public String Complexity()
        {
            return _complexity;
        }

        public String Grade()
        {
            return _grade;
        }

        public int Score()
        {
            return _score;
        }

        public int IsCompromised()
        {
            return _isCompromised;
        }

        public int TimesSeen()
        {
            return _timesSeen;
        }
    }
    public static class Security
    {
        public static Result SimpleCheck(String password)
        {
            return Analyze(password);
        }
        
        public static Result Check(String password)
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
            catch (Exception e) { }
            Password passwordObject = GetInstance();
            return passwordObject.new Result(offlineResult, isCompromized, timesSeen);
        }
        
        private static int[] IsCompromised(String password) throws Exception
        {
            String result = CryptoHelper.SHA1Hash(password).toUpperCase();
            String hashToCheck = result.substring(5);
            // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // get a list of all the possible passwords where the first 5 digits of the hash are the same
            int isCompromised = 0;
            int timesSeen = 0;
            URL url = new URL("https://api.pwnedpasswords.com/range/" + result.substring(0, 5));
            HttpsURLConnection con = (HttpsURLConnection) url.openConnection();
            con.setRequestMethod("GET");
            BufferedReader in = new BufferedReader( new InputStreamReader(con.getInputStream()));
    		String inputLine;
    		List<String> content = new ArrayList<String>();
    		while ((inputLine = in.readLine()) != null) 
    		{
    		    content.add(inputLine);
    		}
    		in.close();
    		con.disconnect();
    		for (int i = 0; i < content.size(); i++) 
    		{
    			String[] parts = content.get(i).split(":");
    			if (parts[0].equals(hashToCheck))
                {
                    isCompromised = 1;
                    timesSeen = Integer.parseInt(parts[1]);
                    break;
                }
    		}
            
            return new int[] { isCompromised, timesSeen };
        }
        private static Result Analyze(String password)
        {
            // Originally created by: Jeff Todnem (http://www.todnem.com/) in Javascript --> http://www.passwordmeter.com/js/pwdmeter.js
            // Ported to C# by Frederik Höft
        	int score = 0, upperCaseCount = 0, lowerCaseCount = 0, numberCount = 0, symbolCount = 0, unicodeCount = 0, middleSpecialCharacterCount = 0, nRequirements = 0, uniqueCharacterCount = 0, repeatedCharacterCount = 0, repeatedCharacterDeduction = 0, consecutiveUpperCaseCount = 0, consecutiveLowerCaseCount = 0, consecutiveNumberCount = 0, sequentialLetterCount = 0, sequentialNumberCount = 0, sequentialSymbolCount = 0, sequentialUnicodeCount = 0, sequentialCharacterCount = 0, fulfilledRequirementsCount = 0;
            int middleSpecialCharacterMultiplier = 2, consecutiveUpperCaseMultiplier = 2, consecutiveLowerCaseMultiplier = 2, consecutiveNumberMultiplier = 2, unicodeMultiplier = 30;
            int sequentialLetterMultiplier = 3, sequentialNumberMultiplier = 3, sequentialSymbolMultiplier = 2, sequentialUnicodeMultiplier = 1;
            int lengthMultipier = 6, numberMultiplier = 4;
            int symbolMultiplier = 8;
            int upperCaseLastIndex = -1, lowerCaseLastIndex = -1, numberLastIndex = -1;
            String alphabet = "abcdefghijklmnopqrstuvwxyz";
            String keyboard = "qwertyuiopasdfghjklzxcvbnm";
            String numbers = "01234567890";
            String symbols = " !\"#$%&\'()*+,-./:;<=>?@[\\]^_{|}~";
            String passwordComplexity = "Too Short";
            int minimumPasswordLength = 12;
            score = password.length() * lengthMultipier;
            String[] passwordArray = password.split("");
            int passwordLength = passwordArray.length;

            /* Loop through password to check for Symbol, Numeric, Lowercase and Uppercase pattern matches */
            for (int a = 0; a < passwordLength; a++)
            {
                if (passwordArray[a].matches("[A-Z]"))
                {
                    // Check if previous character was upper case as well
                    if (upperCaseLastIndex != -1 && (upperCaseLastIndex + 1) == a)
                    {
                        consecutiveUpperCaseCount++;
                    }
                    upperCaseLastIndex = a;
                    upperCaseCount++;
                }
                else if (passwordArray[a].matches("[a-z]"))
                {
                    // Check if previous character was lower case as well
                    if (lowerCaseLastIndex != -1 && (lowerCaseLastIndex + 1) == a)
                    {
                        consecutiveLowerCaseCount++;
                    }
                    lowerCaseLastIndex = a;
                    lowerCaseCount++;
                }
                else if (passwordArray[a].matches("[0-9]"))
                {
                    if (a > 0 && a < (passwordLength - 1))
                    {
                        middleSpecialCharacterCount++;
                    }
                    // Check if previous character was a number as well
                    if (numberLastIndex != -1 && (numberLastIndex + 1) == a)
                    {
                        consecutiveNumberCount++;
                    }
                    numberLastIndex = a;
                    numberCount++;
                }
                else if (symbols.contains(passwordArray[a]))
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
                boolean repeatedCharactersExist = false;
                for (int b = 0; b < passwordLength; b++)
                {
                    if (passwordArray[a].equals(passwordArray[b]) && a != b)
                    { /* repeat character exists */
                        repeatedCharactersExist = true;
                        /* 
                        Calculate increment deduction based on proximity to identical characters
                        Deduction is incremented each time a new match is discovered
                        Deduction amount is based on total password length divided by the
                        difference of distance between currently selected match
                        */
						if (symbols.contains(passwordArray[a]))
						{
							repeatedCharacterDeduction += symbolMultiplier;
						}
						else if (!passwordArray[a].matches("[A-Za-z0-9]"))
						{
							repeatedCharacterDeduction += unicodeMultiplier;
						}
                        repeatedCharacterDeduction += Math.abs(passwordLength / (b - a));
                    }
                }
                if (repeatedCharactersExist)
                {
                    repeatedCharacterCount++;
                    uniqueCharacterCount = passwordLength - repeatedCharacterCount;
                    repeatedCharacterDeduction = (uniqueCharacterCount != 0) ? (int)Math.ceil((double)repeatedCharacterDeduction / uniqueCharacterCount) : (int)Math.ceil((double)repeatedCharacterDeduction);
	            }
            }

            /* Check for sequential alpha String patterns (forward and reverse) */
            for (int s = 0; s < 23; s++)
            {
                String sequence = alphabet.substring(s, s + 3);
                String reversedSequence = new StringBuilder(sequence).reverse().toString();
                String keyboardSequence = keyboard.substring(s, s + 3);
                String reversedKeyboardSequence = new StringBuilder(keyboardSequence).reverse().toString();
                if (password.toLowerCase().contains(sequence.toLowerCase()) || password.toLowerCase().contains(reversedSequence.toLowerCase()) || password.toLowerCase().contains(keyboardSequence) || password.toLowerCase().contains(reversedKeyboardSequence))
                {
                    sequentialLetterCount++;
                    sequentialCharacterCount++;
                }
            }

            /* Check for sequential numeric String patterns (forward and reverse) */
            for (int s = 0; s < numbers.length() - 2; s++)
            {
                String sequence = numbers.substring(s, s + 3);
                String reversedSequence = new StringBuilder(sequence).reverse().toString();
                if (password.contains(sequence) || password.contains(reversedSequence))
                {
                    sequentialNumberCount++;
                    sequentialCharacterCount++;
                }
            }

            /* Check for sequential symbol String patterns (forward and reverse) */
            for (int s = 0; s < symbols.length() - 2; s++)
            {
                String sequence = symbols.substring(s, s + 3);
                String reversedSequence = new StringBuilder(sequence).reverse().toString();
                if (password.contains(sequence) || password.contains(reversedSequence))
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
            for (int c = 0; c < characterCounts.length; c++)
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
            if (password.length() >= minimumPasswordLength) { nMinReqChars = 3; } else { nMinReqChars = 4; }
            if (nRequirements > nMinReqChars)
            {  // One or more required characters exist
                score = score + (nRequirements * 2);
            }

            /* Determine complexity and grade based on overall score */
            String grade = "F";
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
            else if (score > 200)
            {
                grade = "A";
                passwordComplexity = "Very Strong";
            }

            Password passwordObject = GetInstance();
            return passwordObject.new Result(passwordComplexity, grade, score);
        }
    }
}