#!/usr/bin/python

import socket
from socket import error as SocketError
import Crypto
from Crypto import Random
from Crypto.Util import number
from Crypto.Cipher import PKCS1_OAEP, AES
from Crypto.Util.asn1 import DerSequence
from Crypto.PublicKey import RSA
import threading
from threading import Thread
from base64 import standard_b64encode, b64decode
from binascii import a2b_base64
from os.path import basename, exists
from xml.dom import minidom
import ast
import pyscrypt
import sqlite3
import base64
import argparse
import secrets
import hashlib
import os
import glob
import datetime
from datetime import datetime
import time
import subprocess
import select
import re
import errno
import smtplib
import pygeoip
import sys
import getpass
from config import *
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
from email.mime.image import MIMEImage

################################################################################
#-------------------------------GLOBAL VARIABLES-------------------------------#
################################################################################
# ANSI FORMATTINGS
FOVERLINED="\033[53m"
FITALIC="\033[3m"
FUNDERLINED="\033[4m"
FBLINKING="\033[5m"
FCROSSED="\033[9m"
# ANSI COLOR CODES
CBLACK="\033[90m"
CRED="\033[91m"
CGREEN="\033[92m"
CYELLOW="\033[93m"
CBLUE="\033[94m"
CPINK="\033[95m"
CCYAN="\033[96m"
CWHITE="\033[97m"
ENDF="\033[0m"
# VERSION INFO
NAME = "PMDB-Server"
VERSION = "0.10-12.18"
BUILD = "development"
DATE = "Oct 27 2018"
TIME = "20:44:25"
PYTHON = "Python 3.6.6 / LINUX"

################################################################################
#------------------------------SERVER CRYPTO CLASS-----------------------------#
################################################################################


# PROVIDES CRYPTOGRAPHIC METHODS
class CryptoHelper():

	# ENCRYPTS A MESSAGE USING A 4096 BIT RSA KEY
	def RSAEncrypt(plaintext, publicKeyPemString):
		# CREATE RSA KEY OBJECT
		publicKey = RSA.importKey(publicKeyPemString)
		# CREATE CIPHER
		cipher = PKCS1_OAEP.new(publicKey)
		# ENCRYPT MESSAGE
		encryptedBytes = cipher.encrypt(bytes(plaintext, "utf-8"))
		encryptedBytes = base64.b64encode(encryptedBytes)
		# RETURN ENCRYPTED MESSAGE AS BASE64 STRING
		return encryptedBytes.decode("utf-8")
	
	# DECRYPTS A MESSAGE USING A 4096 BIT RSA PRIVATE KEY OBJECT
	def RSADecrypt(cryptotext, privateKey):
		# CREATE CIPHER BASED ON PRIVATE KEY
		cipher = PKCS1_OAEP.new(privateKey)
		# DECRYPT MESSAGE
		decryptedBytes = base64.b64decode(bytes(cryptotext,"utf-8"))
		decryptedBytes = cipher.decrypt(decryptedBytes)
		# RETTURN DECRYPTED MESSAGE AS UTF-8 STRING
		return decryptedBytes.decode("utf-8")
		
	# RETURNS SHA256 HASH OF PLAINTEXT
	def SHA256(plaintext):
		hashBytes = base64.b64encode(hashlib.sha256(bytes(plaintext, "utf-8")).digest())
		return hashBytes.decode("utf-8")
		
	# RETURNS BLAKE2 SALTED HASH OF PLAINTEXT (FAST & SECURE)
	def BLAKE2(plaintext, salt):
		hashBytes = base64.b64encode(hashlib.blake2b(bytes(plaintext + salt, "utf-8")).digest())
		return hashBytes.decode("utf-8")
		
	# RETURNS SCRYPT SALTED HASH OF PLAINTEXT (SLOW & UNCRACKABLE)
	def Scrypt(plaintext, salt):
		return pyscrypt.hash(password = bytes(plaintext, "utf-8"), salt = bytes(salt, "utf-8"), N = 4096, r = 2, p = 1, dkLen = 128).hex()
		
	# GENERATES RSA KEY PAIR
	def RSAKeyPairGenerator():
		# GET A RANDOM SEED
		random_generator = Random.new().read
		# USE RANDOM SEED TO GENERATE RSA OBJECT
		key = RSA.generate(4096, random_generator)
		# GET THE PUBLIC AND PRIVATE KEY FROM RSA OBJECT
		PubKey = key.publickey()
		PrivKey = key
		# RETURN BOTH KEYS (RSA OBJECTS)
		return PubKey, PrivKey

	# CONVERTS PUBLIC RSA KEY FROM PEM FORMAT TO XML
	def RSAPublicPemToXml(pemPublicKey):
		# IMPORT PEM KEY TO RSA OBJECT
		publicKey = RSA.importKey(pemPublicKey)
		# publicKey = pemPublicKey
		# CONSTRUCT XML-FORMATTED KEY STEP-BY-STEP
		xml  = '<RSAParameters>'
		xml += '<Modulus>'
		# EXPORT THE MODULUS FROM RSA OBJECT AND ADD IT TO XML STRING
		xml += standard_b64encode(number.long_to_bytes(publicKey.n)).decode("utf-8")
		xml += '</Modulus>'
		xml += '<Exponent>'
		# EXPORT THE EXPONENT FROM RSA OBJECT AND ADD IT TO XML STRING
		xml += standard_b64encode(number.long_to_bytes(publicKey.e)).decode("utf-8")
		xml += '</Exponent>'
		xml += '</RSAParameters>'
		# RETURN THE PUBLIC XML KEY-STRING
		return xml
	
	# GENERATES 256 BYTE AES KEY
	def AESKeyGenerator():
		# GET 512 RANDOM BYTES
		randomBytes = CryptoHelper.GetRandomBytes(512)
		# CREATE SHA 256 HASH AND ENCODE TO BASE64
		keyBytes = base64.b64encode(hashlib.sha256(randomBytes).digest())
		# DECODE KEY BYTES TO UTF-8 STRING
		AESKeyString = keyBytes.decode("utf-8")
		# RETURN AES KEY
		return AESKeyString
		
	# CONVERTS PUBLIC RSA KEY FROM XML FORMAT TO PEM
	def RSAPublicXmlToPem(xmlPublicKey):
		# PARSE XML-KEY-STRING TO minidom XML-OBJECT
		rsaKeyValue = minidom.parseString(xmlPublicKey)
		# EXTRACT MODULUS AND EXPONENT FROM minidom
		modulus = CryptoHelper.GetLong(rsaKeyValue.getElementsByTagName('Modulus')[0].childNodes)
		exponent = CryptoHelper.GetLong(rsaKeyValue.getElementsByTagName('Exponent')[0].childNodes)
		# CONSTRUCT RSA KEY FROM MODULUS AND EXPONENT
		publicKey = RSA.construct((modulus, exponent))
		# RETURN THE PUBLIC PEM KEY-STRING
		return publicKey.exportKey().decode("utf-8")
		
	#------------------------------HELPER FUNCTIONS----------------------------#
	# GET LONG INTEGER
	def GetLong(nodelist):
		rc = []
		for node in nodelist:
			if node.nodeType == node.TEXT_NODE:
				rc.append(node.data)
		string = ''.join(rc) 
		return number.bytes_to_long(b64decode(string))
	
	# GET RANDOM BYTES 
	def GetRandomBytes(length):
		randomBytes = secrets.token_bytes(length)
		return randomBytes

# AES CIPHER CLASS
class AESCipher(object):
	# INIT FUNCTION (HASH THE KEY/SET BLOCKSIZE)
	def __init__(self, key): 
		# SET BLOCKSIZE TO 32 BYTES (256 BITS)
		self.bs = 32
		# HASH THE PROVIDED KEY USING SHA 256
		self.key = hashlib.sha256(key.encode()).digest()
		
	# ENCRYPT FUNCTION
	def encrypt(self, raw):
		# ENCODE UTF-8 FOR UNICODE CHARACTERS
		raw = raw.encode("utf-8")
		# GET PADDING RIGHT
		raw = self._pad(raw)
		# CREATE RANDOM IV
		iv = Random.new().read(AES.block_size)
		# CREATE CIPHER 
		cipher = AES.new(self.key, AES.MODE_CBC, iv)
		# ENCRYPT + APPEND ENCRYPTED MESSAGE TO IV AND CONVERT TO BASE64 STRING
		return base64.b64encode(iv + cipher.encrypt(raw)).decode("utf-8")
		
	# DECRYPT FUNCTION
	def decrypt(self, enc):
		# DECODE BASE64 STRING TO BYTES
		enc = base64.b64decode(enc)
		# READ IV FROM BYTES
		iv = enc[:AES.block_size]
		# CREATE AES CIPHER
		cipher = AES.new(self.key, AES.MODE_CBC, iv)
		# DECRYPT MESSAGE + CONVERT TO UTF-8 STRING
		return self._unpad(cipher.decrypt(enc[AES.block_size:])).decode("utf-8")

	def _pad(self, s):
		return s + (self.bs - len(s) % self.bs) * chr(self.bs - len(s) % self.bs).encode("utf-8")

	@staticmethod
	def _unpad(s):
		return s[:-ord(s[len(s)-1:])]

# PROVIDES DATABASE RELATED METHODS
class DatabaseManagement():
	
	# INSERT DATA 
	def Insert(command, clientAddress, clientSocket, aesKey):
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		try:
			# SPLIT THE RAW COMMAND TO GET THE CREDENTIALS
			# EXAMPLE COMMAND
			# localID\x1funame&eq!uname!;password%eq!password!;...
			parameters = command.split('\x1f')
			# GET THE CREDENTIALS FROM ARRAY
			queryParameter = parameters[0]
			localID = parameters[1]
			# CHECK FOR SECURITY ISSUES
			if not DatabaseManagement.Security(parameters, clientAddress, clientSocket, aesKey):
				return
			# SECURITY CHECK PASSED
			# CHECK CREDENTIALS
			userID = Management.CheckCredentials(clientAddress, clientSocket, aesKey)
			if not userID:
				Handle.Error("NLGI", None, clientAddress, clientSocket, aesKey, True)
				return
			# CREDENTIAL CHECK PASSED
			# FORMAT PARAMETERS
			queryParameters = queryParameter.split(";")
			# INITIALIZE VARAIBLES
			qUsername = None
			qPassword = None
			qHost = None
			qEmail = None
			qNotes = None
			query = ""
			values = ""
			# ITERATE OVER QUERY PARAMETERS AND SET VARIABLES
			for rawParameter in queryParameters:
				if not len(rawParameter) == 0:
					parameters = rawParameter.split("%eq")
					column = parameters[0]
					value = parameters[1].replace('!','')
					# CHECK IF CULUMN IS VALID
					if column in ["uname","password","host","notes","email","datetime","url"]:
						if query == "":
							query = "D_" + column
							values = "\"" + value + "\""
						else:
							query += ",D_" + column
							values += ",\"" + value + "\""
			# CHECK IF ANY PARAMETERS HAVE BEEN SET
			if query == "":
				# THROW INVALID SQL QUERY PARAMETERS EXCEPTION
				Handle.Error("ISQP", None, clientAddress, clientSocket, aesKey, True)
				return
			# CHECK PASSED: REQUEST IS VALID
			# APPEND USER ID TO QUERY
			query += "," + "D_userid"
			values += "," + str(userID)
			# EXECUTE SQL QUERY
			fullQuery = "INSERT INTO Tbl_data (" + query + ") VALUES (" + values + ");"
			# PrintSendToAdmin(fullQuery)
			cursor.execute(fullQuery)
			# COMMIT CHANGES
			connection.commit()
			cursor.execute("SELECT last_insert_rowid() FROM Tbl_data;")
			# CREATE HID (HASHED ID)
			dataID = cursor.fetchall()
			if len(dataID) == 0:
				return
			HID = dataID[0][0]
			hashedID = CryptoHelper.BLAKE2(str(HID), str(userID))
			# ADD HID TO ENTRY
			cursor.execute("UPDATE Tbl_data SET d_hid = \"" + hashedID + "\" WHERE D_id = " + str(HID) + ";")
			connection.commit()
			Log.ClientEventLog("INSERT", clientSocket)
			# SEND ACKNOWLEDGEMENT TO CLIENT
			aesEncryptor = AESCipher(aesKey)
			# ENCRYPT DATA
			encryptedData = aesEncryptor.encrypt("DTARETINS" + localID + "\x1f" + hashedID)
			PrintSendToAdmin("SERVER ---> RETURNED STATUS            ---> " + clientAddress)
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		finally:
			# FREE RESOURCES
			connection.close()
		
	# UPDATE DATA	
	def Update(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE command = "host%eq!test!;uname%eq!test!;password%eq!test!;email%eq!test!;notes%eq!test!;datetime%eq!test!;hid%eq!test!\x1f12;"
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		try:
			# SPLIT THE RAW COMMAND TO GET THE parameters
			parameters = command.split('\x1f')
			# GET THE CREDENTIALS FROM ARRAY
			queryParameter = parameters[0]
			localID = parameters[1]
			# CHECK FOR SECURITY ISSUES
			if not DatabaseManagement.Security(parameters, clientAddress, clientSocket, aesKey):
				return
			# SECURITY CHECK PASSED
			# CHECK CREDENTIALS
			userID = None
			userID = Management.CheckCredentials(clientAddress, clientSocket, aesKey)
			if userID == None:
				Handle.Error("NLGI", None, clientAddress, clientSocket, aesKey, True)
				return
			# CREDENTIAL CHECK PASSED
			# FORMAT PARAMETERS
			queryParameters = queryParameter.split(";")
			query = ""
			HID = None
			# ITERATE OVER QUERY PARAMETERS AND SET VARIABLES
			for rawParameter in queryParameters:
				if not len(rawParameter) == 0:
					parameters = rawParameter.split("%eq")
					column = parameters[0]
					value = parameters[1].replace('!','')
					if column in ["uname","password","host","notes","email","datetime"]:
						if query == "":
							query = "D_" + column + " = \"" + value + "\""
						else:
							query += ",D_" + column + " = \"" + value + "\""
					elif column == "hid":
						HID = value
			# CHECK IF ANY PARAMETERS HAVE BEEN SET
			if query == "":
				# THROW INVALID SQL QUERY PARAMETERS EXCEPTION
				Handle.Error("ISQP", None, clientAddress, clientSocket, aesKey, True)
				return
			# CHECK PASSED: REQUEST IS VALID
			# CONCATENATE AND EXECUTE QUERY
			fullQuery = "UPDATE Tbl_data SET " + query + " WHERE D_userid = " + userID + " AND d_hid = \"" + HID + "\";"
			#PrintSendToAdmin(fullQuery)
			cursor.execute(fullQuery)
			# COMMIT CHANGES
			connection.commit()
			Log.ClientEventLog("UPDATE", clientSocket)
			# SEND ACKNOWLEDGEMENT TO CLIENT
			aesEncryptor = AESCipher(aesKey)
			# ENCRYPT DATA
			encryptedData = aesEncryptor.encrypt("DTARETUPD" + localID + "\x1f" + "ACK")
			PrintSendToAdmin("SERVER ---> RETURNED STATUS            ---> " + clientAddress)
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		finally:
			# FREE RESOURCES
			connection.close()
		
	# SELECT DATA FROM THE DATABASE
	def Select(queryParameter, clientAddress, clientSocket, aesKey):
		# EXAMPLE command = "HID1;HID2;HID3;HID4;HID5;"
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		try:
			# CHECK FOR SECURITY ISSUES
			queryParameters = queryParameter.split(";")
			if not DatabaseManagement.Security(queryParameters, clientAddress, clientSocket, aesKey):
				return
			# SECURITY CHECK PASSED
			# CHECK CREDENTIALS
			userID = None
			userID = Management.CheckCredentials(clientAddress, clientSocket, aesKey)
			if userID == None:
				Handle.Error("NLGI", None, clientAddress, clientSocket, aesKey, True)
				return
			# CREDENTIAL CHECK PASSED
			# FORMAT PARAMETERS
			query = ""
			# ITERATE OVER QUERY PARAMETERS AND SET VARIABLES
			for parameter in queryParameters:
				if not len(parameter) == 0:
					if query == "":
						query = "d_hid = \"" + parameter + "\""
					else:
						query += " OR d_hid = \"" + parameter + "\""
			# CHECK IF ANY PARAMETERS HAVE BEEN SET
			if query == "":
				# THROW INVALID SQL QUERY PARAMETERS EXCEPTION
				Handle.Error("ISQP", None, clientAddress, clientSocket, aesKey, True)
				return
			# CHECK PASSED: REQUEST IS VALID
			cursor.execute("SELECT * FROM Tbl_data WHERE D_userid = " + userID + " AND (" + query + ");")
			# GET DATA FROM CURSOR OBJECT
			rawData = cursor.fetchall()
			# COMMIT CHANGES
			connection.commit()
			# CREATE AND SEND PACKET FOR EACH RETURNED ROW
			for entry in rawData:
				# GET VALUES FROM DATA ARRAY
				dHost = str(entry[1])
				dUname = str(entry[2])
				dPassword = str(entry[3])
				dEmail = str(entry[4])
				dNotes = str(entry[5])
				dHid = str(entry[7])
				dDatetime = str(entry[8])
				# APPLY PACKET FORMATTING
				# EXAMPLE: host%eq!test!;uname%eq!test!;password%eq!test!;email%eq!test!;notes%eq!test!;hid%eq!test!;datetime%eq!test!;
				data = "host%eq!" + dHost + "!;uname%eq!" + dUname + "!;password%eq!" + dPassword + "!;email%eq!" + dEmail + "!;notes%eq!" + dNotes + "!;hid%eq!" + dHid + "!;datetime%eq!" + dDatetime + "!;"
				# RETURN DATA TO CLIENT
				aesEncryptor = AESCipher(aesKey)
				# ENCRYPT DATA
				encryptedData = aesEncryptor.encrypt("DTARETSEL" + data)
				PrintSendToAdmin("SERVER ---> RETURNED DATA              ---> " + clientAddress)
				clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
			Log.ClientEventLog("SELECT", clientSocket)
			# SEND ACKNOWLEDGEMENT TO CLIENT (LAST PACKET OUT)
			aesEncryptor = AESCipher(aesKey)
			# ENCRYPT DATA
			encryptedData = aesEncryptor.encrypt("INFRETSELACK")
			PrintSendToAdmin("SERVER ---> RETURNED STATUS            ---> " + clientAddress)
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		finally:
			# FREE RESOURCES
			connection.close()
	
	def Delete(queryParameter, clientAddress, clientSocket, aesKey):
		# EXAMPLE command = "HID1;HID2;HID3;HID4;HID5;"
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		try:
			queryParameters = queryParameter.split(";")
			# CHECK FOR SECURITY ISSUES
			if not DatabaseManagement.Security(queryParameters, clientAddress, clientSocket, aesKey):
				return
			# SECURITY CHECK PASSED
			# CHECK CREDENTIALS
			userID = None
			userID = Management.CheckCredentials(clientAddress, clientSocket, aesKey)
			if userID == None:
				Handle.Error("NLGI", None, clientAddress, clientSocket, aesKey, True)
				return
			# CREDENTIAL CHECK PASSED
			# FORMAT PARAMETERS
			query = ""
			# ITERATE OVER QUERY PARAMETERS AND SET VARIABLES
			for parameter in queryParameters:
				if not len(parameter) == 0:
					if query == "":
						query = "d_hid = \"" + parameter + "\""
					else:
						query += " OR d_hid = \"" + parameter + "\""
			# CHECK IF ANY PARAMETERS HAVE BEEN SET
			if query == "":
				# THROW INVALID SQL QUERY PARAMETERS EXCEPTION
				Handle.Error("ISQP", None, clientAddress, clientSocket, aesKey, True)
				return
			# CHECK PASSED: REQUEST IS VALID
			cursor.execute("DELETE FROM Tbl_data WHERE D_userid = " + userID + " AND (" + query + ");")
			connection.commit()
			Log.ClientEventLog("DELETE", clientSocket)
			# SEND ACKNOWLEDGEMENT TO CLIENT
			aesEncryptor = AESCipher(aesKey)
			# ENCRYPT DATA
			encryptedData = aesEncryptor.encrypt("DTARETDELCONFIRMED" + queryParameter)
			PrintSendToAdmin("SERVER ---> RETURNED STATUS            ---> " + clientAddress)
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		finally:
			# FREE RESOURCES
			connection.close()
			
	def Sync(queryParameter, clientAddress, clientSocket, aesKey):
		# EXAMPLE command = "fetch_mode%eq!FETCH_SYNC!"
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		try:
			# CHECK FOR SECURITY ISSUES
			if not DatabaseManagement.Security([queryParameter], clientAddress, clientSocket, aesKey):
				return
			# SECURITY CHECK PASSED
			# CHECK CREDENTIALS
			userID = Management.CheckCredentials(clientAddress, clientSocket, aesKey)
			if not userID:
				Handle.Error("NLGI", None, clientAddress, clientSocket, aesKey, True)
				return
			# CREDENTIAL CHECK PASSED
			# CHECK IF THERE IS NOT MORE THAN 1 PARAMETER
			if not queryParameter.count("%eq") == 1:
				# THROW INVALID SQL QUERY PARAMETERS EXCEPTION
				Handle.Error("ISQP", None, clientAddress, clientSocket, aesKey, True)
				return
			# PARAMETER CHECK PASSED
			# GENERATE SQL QUERY
			fetchMode = queryParameter.split("%eq")[1].replace('!','')
			# SYNC-MODE ONLY FETCHES IDS 
			if fetchMode == "FETCH_SYNC":
				cursor.execute("SELECT D_hid,D_datetime FROM Tbl_data WHERE D_userid = " + userID + ";")
				data = cursor.fetchall()
				finalData = str(data).replace(", ",",").replace('[','').replace(']','')
				# SEND ACKNOWLEDGEMENT TO CLIENT (LAST PACKET OUT)
				aesEncryptor = AESCipher(aesKey)
				# ENCRYPT DATA
				encryptedData = aesEncryptor.encrypt("DTARETSYN" + finalData)
				PrintSendToAdmin("SERVER ---> RETURNED SYNDATA           ---> " + clientAddress)
				clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
				Log.ClientEventLog("SYNC", clientSocket)
			# DUMP-MODE RETURNS ALL DATA
			elif fetchMode == "FETCH_ALL":
				cursor.execute("SELECT * FROM Tbl_data WHERE D_userid = " + userID + ";")
				data = cursor.fetchall()
				finalData = str(data).replace(", ",",").replace('[','').replace(']','')
				# SEND ACKNOWLEDGEMENT TO CLIENT (LAST PACKET OUT)
				aesEncryptor = AESCipher(aesKey)
				# ENCRYPT DATA
				encryptedData = aesEncryptor.encrypt("DTARETSYN" + finalData)
				PrintSendToAdmin("SERVER ---> RETURNED DTADUMP           ---> " + clientAddress)
				clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
				Log.ClientEventLog("DATA_DUMP", clientSocket)
			else:
				# THROW INVALID SQL QUERY PARAMETERS EXCEPTION
				Handle.Error("ISQP", "INVALID_MODE", clientAddress, clientSocket, aesKey, True)
				return
		finally:
			# FREE RESOURCES
			connection.close()
		
	# CHECKS FOR SECURITY ISSUES
	def Security(queryArray, clientAddress, clientSocket, aesKey):
		# CHECK FOR SQL INJECTION
		for element in queryArray:
			if ('\"' in element) or ('\'' in element):
				Handle.Error("SQLI", None, clientAddress, clientSocket, aesKey, True)
				Log.ClientEventLog("SQL_INJECTION_ATTEMPT", clientSocket)
				Log.ServerEventLog("SQL_INJECTION_ATTEMPT", clientAddress)
				Management.Logout(clientAddress, clientSocket, aesKey, True)
				Management.Disconnect(clientSocket, "ANTI_SQL_INJECTION", clientAddress, False)
				return False
		# SQL INJECTION CHECK PASSED
		return True
	
	# CHECKS FOR SECURITY ISSUES WITHOUT HANDLING ERRORS OR WRITING LOGS
	def SecuritySilent(queryArray):
		# CHECK FOR SQL INJECTION
		for element in queryArray:
			if ('\"' in element) or ('\'' in element):
				return False
		# SQL INJECTION CHECK PASSED
		return True
		
# METHODS RELATED TO CREATING AND MANAGING LOGS		
class Log():

	# SCANS THE CLIENTS PORTS USING NMAP TO GET DETAILS FOR LOGGING
	def GetDetails(address, clientSocket):
		if "(" in address:
			address = address.replace("(","!").replace(")","!").split("!")[1]
		details = ""
		if ":" in address:
			details = "IP: " + address.split(":")[0] + "\nPort: " + address.split(":")[1]
			address = address.split(":")[0]
		else:
			details = "IP: " + address
		# CHECK IF SERVER SUPPORTS NMAP SCANS
		if Server.nmap:
			# USE COMMON PORTS FOR OS DETECTION
			command = ["nmap", "-p", "22,80,445,65123,56123,54674", "-O", address]
			# START NMAP IN SUBPROCESS
			resultArray = subprocess.Popen(command, stdout=subprocess.PIPE).communicate()[0].decode("utf-8").split("\n")
			# PARSE NMAP RESULTS
			for info in resultArray:
				if "MAC Address:" in info:
					details += "\n" + info
				elif "Running:" in info or "Aggressive OS guesses:" in info:
					details += "\n" + info.replace("Running:","OS:")
				elif "Nmap scan report for" in info:
					details += "\nDNS: " + info.split(" ")[4]
				elif "Host is up" in info:
					details += "\nPing: " + info.split(" ")[3].replace("(","")
		if Server.geolocatingAvailable:
			NetLocator = pygeoip.GeoIP("GeoDataBases/GeoIPASNum.dat", pygeoip.MEMORY_CACHE)
			GeoLocator = pygeoip.GeoIP("GeoDataBases/GeoLiteCity.dat", pygeoip.MEMORY_CACHE)
			isLocal = False
			try:
				isp = str(NetLocator.isp_by_addr(address))
				country = str(GeoLocator.country_name_by_addr(address))
				data = str(GeoLocator.record_by_addr(address)).split(",")
			except:
				isLocal = True
			if isLocal:
				details += "\nLocation: local network"
			else:
				postalCode = None
				continent = None
				city = None
				timeZone = None
				for info in data:
					if not " None" in info:
						if "postal_code" in info:
							postalCode = "\nPostal code: " + info.split("\'")[3]
						elif "continent" in info:
							continent = "\nContinent: " + info.split("\'")[3]
						elif "city" in info:
							city = "\nCity: " + info.split("\'")[3]
						elif "time_zone" in info:
							timeZone = "\nTime zone: " + info.split("\'")[3]
				if isp:
					details += "\nISP: " + isp
				if continent:
					details += continent
				if timeZone:
					details += timeZone
				if country:
					details += "\nCountry: " + country
				if city:
					details += city
				if postalCode:
					details += postalCode
		# USE SOME FANCY FORMATTING FOR THE OUTPUT
		PrintSendToAdmin(CWHITE + "┌─[" + CRED + "DETAILS FOR " + address + CWHITE + "]" + ENDF)
		detailArray = details.split("\n")
		detailCount = len(detailArray)
		for index, detail in enumerate(detailArray):
			if not index == (detailCount - 1):
				PrintSendToAdmin(CWHITE + "├─╼ " + detail + ENDF)
			else:
				PrintSendToAdmin(CWHITE + "└─╼ " + detail + ENDF)
		# ADD DETAILS TO CLIENT PROFILE
		for index, client in enumerate(Server.allClients):
			if clientSocket in client and len(client) == 3:
				Server.allClients[index].append(details)
	
	# CREATES LOG FOR CLIENT RELATED EVENTS 
	def ClientEventLog(event, clientSocket):
		# CHECKS FOR SQL INJECTION
		sqliSecure = DatabaseManagement.SecuritySilent([event])
		if not sqliSecure:
			return False
		# INITIALIZE VARIABLES
		dateTime = Timestamp()
		address = None
		userID = None
		details = None
		#allClients = [] #[[socket, address, adminFlag, details],[...]]
		#authorizedClients = [] # [[ID, socket, username],[...]]
		# GET USER ID
		for client in Server.authorizedClients:
			if clientSocket in client:
				userID = client[0]
		if not userID:
			return False
		# GET ADDRESS AND DETAILS
		for client in Server.allClients:
			if clientSocket in client:
				address = client[1]
				try:
					details = client[3]
				except:
					pass
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# WRITE ENTRY TO DATABASE
		try:
			cursor.execute("INSERT INTO Tbl_clientLog (L_event, L_ip, L_datetime, L_details, L_userid) VALUES (\"" + event + "\",\"" + address + "\",\"" + dateTime + "\",\"" + details + "\"," + userID + ");")
		except:
			connection.rollback()
			return False
		else:
			connection.commit()
			return True
		finally:
			# FREE RESOURCES
			connection.close()
	
	# CREATES LOG FOR SERVER RELATED OR GLOBAL EVENTS
	def ServerEventLog(event, details):
		# CHECKS FOR SQL INJECTION
		sqliSecure = DatabaseManagement.SecuritySilent([event, details])
		if not sqliSecure:
			return False
		# GET TIMESTAMP
		dateTime = Timestamp()
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# WRITE ENTRY TO DATABASE
		try:
			cursor.execute("INSERT INTO Tbl_serverLog (S_event, S_datetime, S_details) VALUES(\"" + event + "\",\"" + dateTime + "\",\"" + details + "\");")
			connection.commit()
			return True
		except:
			return False
		finally:
			# FREE RESOURCES
			connection.close()
		
class Handle():
	
	def Error(errorID, message, clientAddress, clientSocket, aesKey, isEncrypted):
		#---------------------------------ERROR CODES----------------------------------#
		#
		# [ERRNO 00] UNKN				--> UNKNOWN ERROR
		# [ERRNO 01] IEOT				--> INVALID PACKET TERMINATOR
		# [ERRNO 02] IRSA				--> INVALID RSA KEY
		# [ERRNO 03] USEC				--> UNSECURE CONNECTION
		# [ERRNO 04] IPID				--> INVALID PACKET ID
		# [ERRNO 05] IPSP				--> INVALID PACKET SPECIFIER
		# [ERRNO 06] ISID				--> INVALID PACKET SUB ID
		# [ERRNO 07] SQLI				--> SQL INJECTION ATTEMPT
		# [ERRNO 08] CRED				--> INVALID CREDENTIALS
		# [ERRNO 09] ISQP				--> INVALID SQL QUERY PARAMETERS
		# [ERRNO 10] ADMN				--> INVALID ADMIN CREDENTIALS
		# [ERRNO 11] ACNA				--> ADMIN ALREADY LOGGED IN
		# [ERRNO 12] PERM				--> INSUFFICIENT PERMISSIONS
		# [ERRNO 13] NLGI				--> (LOGOUT ERROR) NOT LOGGED IN
		# [ERRNO 14] ISOH				--> INVALID START OF HEADER
		# [ERRNO 15] ICMD				--> INVALID COMMAND
		# [ERRNO 16] NFND				--> NOT FOUND
		# [ERRNO 17] SQLE				--> SQL	ERROR
		# [ERRNO 18] I2FA				--> INVALID 2FA CODE
		# [ERRNO 19] E2FA				--> EXPIRED 2FA CODE
		# [ERRNO 20] F2FA				--> FAILED 2FA (3 TIMES WRONG CODE)
		# [ERRNO 21] NCES				--> NO CODE EVENT SCHEDULED
		# [ERRNO 22] UEXT				--> USER ALREADY EXISTS
		# [ERRNO 23] CDNE				--> COOKIE DOES NOT EXIST
		# [ERRNO 24] DVFY				--> VERIFY NEW DEVICE
		# [ERRNO 25] UDNE				--> USER DOES NOT EXIST
		# [ERRNO 26] ACCB				--> ACCOUNT BANNED
		# [ERRNO 27] CRYP				--> CRYPTOGRAPHIC EXCEPTION
		#
		#------------------------------------------------------------------------------#
		errorNo = None
		if errorID == "UNKN":
			errorNo = "00"
			if not message:
				message = "UNKNOWN_ERROR"
		elif errorID == "IEOT":
			errorNo = "01"
			if not message:
				message = "INVALID_PACKET_TERMINATOR"
		elif errorID == "IRSA":
			errorNo = "02"
			if not message:
				message = "INVALID_RSA_KEY"
		elif errorID == "USEC":
			errorNo = "03"
			if not message:
				message = "UNSECURE_CONNECTION"
		elif errorID == "IPID":
			errorNo = "04"
			if not message:
				message = "INVALID_PACKET_ID"
		elif errorID == "IPSP":
			errorNo = "05"
			if not message:
				message = "INVALID_PACKET_SPECIFIER"
		elif errorID == "ISID":
			errorNo = "06"
			if not message:
				message = "INVALID_PACKET_SUB_ID"
		elif errorID == "SQLI":
			errorNo = "07"
			if not message:
				message = "SQL_INJECTION_CHECK_FAILED"
		elif errorID == "CRED":
			errorNo = "08"
			if not message:
				message = "INVALID_CREDENTIALS"
		elif errorID == "ISQP":
			errorNo = "09"
			if not message:
				message = "INVALID_SQL_QUERY_PARAMETERS"
		elif errorID == "ADMN":
			errorNo = "10"
			if not message:
				message = "INVALID_ADMIN_CREDENTIALS"
		elif errorID == "ACNA":
			errorNo = "11"
			if not message:
				message = "ADMIN_ALREADY_LOGGED_IN"
		elif errorID == "PERM":
			errorNo = "12"
			if not message:
				message = "INSUFFICIENT_PERMISSIONS"
		elif errorID == "NLGI":
			errorNo = "13"
			if not message:
				message = "NOT_LOGGED_IN"
		elif errorID == "ISOH":
			errorNo = "14"
			if not message:
				message = "INVALID_START_OF_HEADER"
		elif errorID == "ICMD":
			errorNo = "15"
			if not message:
				message = "INVALID_COMMAND"
		elif errorID == "NFND":
			errorNo = "16"
			if not message:
				message = "NOT_FOUND"
		elif errorID == "SQLE":
			errorNo = "17"
			if not message:
				message = "SQL_ERROR"
		elif errorID == "I2FA":
			errorNo = "18"
			if not message:
				message = "INVALID_2FA_CODE"
		elif errorID == "E2FA":
			errorNo = "19"
			if not message:
				message = "EXPIRED_2FA_CODE"
		elif errorID == "F2FA":
			errorNo = "20"
			if not message:
				message = "FAILED_2FA"
		elif errorID == "NCES":
			errorNo = "21"
			if not message:
				message = "NO_CODE_EVENT_SCHEDULED"
		elif errorID == "UEXT":
			errorNo = "22"
			if not message:
				message = "USER_ALREADY_EXISTS"
		elif errorID == "CDNE":
			errorNo = "23"
			if not message:
				message = "COOKIE_DOES_NOT_EXIST"
		elif errorID == "DVFY":
			errorNo = "24"
			if not message:
				message = "VERIFY_NEW_DEVICE"
		elif errorID == "UDNE":
			errorNo = "25"
			if not message:
				message = "USER_DOES_NOT_EXIST"
		elif errorID == "ACCB":
			errorNo = "26"
			if not message:
				message = "ACCOUNT_BANNED"
		elif errorID == "CRYP":
			errorNo = "27"
			if not message:
				message = "CRYPTOGRAPHIC_EXCEPTION"
		else:
			return
		info = "errno%eq!" + errorNo + "!;code%eq!" + errorID + "!;message%eq!" + str(message) +"!;"
		Log.ServerEventLog("Error", info)
		PrintSendToAdmin("SERVER <-#- [ERRNO " + errorNo + "] " + errorID + "            -#-> " + clientAddress)
		if not isEncrypted:
			clientSocket.send(b'\x01' + bytes("UINFERR" + info, "utf-8") + b'\x04')
		aesEncryptor = AESCipher(aesKey)
		encryptedData = aesEncryptor.encrypt("INFERR" + info)
		clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		
class Management():
	
	# WHITELISTS CLIENT AS ADMIN AFTER VALIDATING PROVIDED 2FA CODE
	def LoginNewAdmin(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE COMMAND
		# code%eq!code!;password%eq!password!;cookie%eq!cookie!;
		creds = command.split(";")
		# CHECK FOR SQL INJECTION
		if not DatabaseManagement.Security(creds, clientAddress, clientSocket, aesKey):
			return
		# SECURITY CHECK PASSED
		# INITIALIZE VARIABLES
		password = None
		cookie = None
		providedCode = None
		# EXTRACT VALUES FROM PROVIDED COMMAND
		try:
			for credential in creds:
				if "password" in credential:
					password = credential.split("!")[1]
				elif "cookie" in credential:
					cookie = credential.split("!")[1]
				elif "code" in credential:
					providedCode = credential.split("!")[1]
				elif len(credential) == 0:
					pass
				else:
					# COMMAND CONTAINS MORE DATA THAN REQUESTED --> THROW INVALID COMMAND EXCEPTION
					Handle.Error("ICMD", "TOO_MANY_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
					return
		except Exception as e:
			# COMMAND HAS UNKNOWN FORMATTING --> THROW INVALID COMMAND EXCEPTION
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			return
		# VALIDATE THAT ALL VARIABLES HAVE BEEN INITIALIZED
		if not providedCode or not password or not cookie:
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
			return
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# INITALIZE VARIABLES TO STORE VALUES FOR CODE VALIDATION
		code = None
		codeTime = None
		codeType = None
		codeAttempts = None
		try:
			# QUERY DAATABASE FOR VALIDATION CODES
			cursor.execute("SELECT U_code, U_codeTime, U_codeType, U_codeAttempts FROM Tbl_user WHERE U_username = \"__ADMIN__\";")
			data = cursor.fetchall()
			code = data[0][0]
			codeTime = data[0][1]
			codeType = data[0][2]
			codeAttempts = data[0][3]
		except Exception as e:
			# SOMETHING SQL RELATED WENT WRONG / MIGHT ALSO BE INDEX OUT OF RANGE --> THROW EXCEPTION
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF ALL VALIABLES HAVE BEEN INITIALIZED
		if not code or not codeTime or codeAttempts == None or not codeType:
			Handle.Error("SQLE", "VALIABLES_NOT_INITIALIZED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF NEW LOGIN HAS BEEN SCHEDULED
		if not codeType == "NEW_LOGIN" or codeAttempts == -1:
			Handle.Error("NCES", "NO_NEW_LOGIN_SCHEDULED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF VALIDATION CODE MATCHES PROVIDED CODE
		if not code == providedCode:
			# CHECK IF NUMBER OF WRONG CODES HAS BEEN EXCEEDED
			if codeAttempts + 1 >= 3:
				# USER TRIES TO BRUTEFORCE VALIDATION CODE --> 1 HOUR BAN BY MAC ADDRESS OR IP
				Handle.Error("F2FA", None, clientAddress, clientSocket, aesKey, True)
				# BAN DEVICE FOR 1H
				Management.Ban("ip%eq!" + clientAddress.split(":")[0] + "!;duration%eq!3600!;", clientAddress, clientSocket, aesKey, True)
				return
			else:
				# INCREMENT COUNTER FOR WRONG ATTEMPTS
				codeAttempts += 1
				try:
					# UPDATE COUNTER IN DATABASE
					cursor.execute("UPDATE Tbl_user Set U_codeAttempts = " + str(codeAttempts) + " WHERE U_username = \"__ADMIN__\";")
				except Exception as e:
					# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION, ROLLBACK AND FREE RESOURCES
					connection.rollback()
					connection.close()
					Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
					return
				else:
					# SUCCESSFULLY UPDATED DATABASE --> COMMIT CHANGES
					connection.commit()
					Handle.Error("I2FA", None, clientAddress, clientSocket, aesKey, True)
					return
		# CHECK IF VALIDATION CODE HAS EXPIRED
		if int(Timestamp()) - int(codeTime) > 1800:
			# CODE IS OLDER THAN 30 MINUTES AND THEREFORE INVALID
			Handle.Error("E2FA", None, clientAddress, clientSocket, aesKey, True)
			return
		# CANCEL ACTIVE CODE
		try:
			cursor.execute("UPDATE Tbl_user SET U_codeAttempts = -1, U_codeType = \"NONE\";")
		# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
		except Exception as e:
			connection.rollback()
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# ACCOUNT SUCCESSFULLY VERIFIED --> COMMIT CHANGES
		else:
			connection.commit()
		# ALL CODE CHECKS PASSED
		# HASH PASSWORD
		hashedUsername = CryptoHelper.SHA256("__ADMIN__")
		salt = CryptoHelper.SHA256(hashedUsername + password)
		hashedPassword = CryptoHelper.Scrypt(password, salt)
		userID = None
		try:
			# CHECK IF CREDENTIALS ARE VALID
			# QUERY FOR USER ID
			cursor.execute("SELECT U_id FROM Tbl_user WHERE U_username = \"__ADMIN__\" AND U_password = \"" + hashedPassword + "\";")
			# FETCH DATA
			data = cursor.fetchall()
			# GET USER ID
			userID = str(data[0][0])
		except:
			connection.close()
			Log.ClientEventLog("LOGIN_ATTEMPT_FAILED", clientSocket)
			# RETURN ERROR MESSAGE TO CLIENT
			Handle.Error("ADMN", None, clientAddress, clientSocket, aesKey, True)
			return
		try:
			# CREATE CONNECTION BETWEEN ACCOUNT ID AND COOKIE ID
			cursor.execute("INSERT INTO Tbl_connectUserCookies (U_id, C_id) VALUES (" + userID + ", (SELECT C_id FROM Tbl_cookies WHERE C_cookie = \"" + cookie + "\"));")
		except Exception as e:
			# SQL ERROR --> ROLLBACK
			connection.rollback()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES 
			connection.commit()
		finally:
			# FREE RESOURCES
			connection.close()
		# CREDENTIAL CHECK PASSED
		# CHECK FOR OTHER ADMINS
		if not Server.admin == None:
			# THROW ADMIN ALREADY LOGGED IN
			# RETURN ERROR MESSAGE TO CLIENT
			Handle.Error("ACNA", None, clientAddress, clientSocket, aesKey, True)
			return
		Management.Logout(clientAddress, clientSocket, aesKey, False)
		# SET UP ADMIN STATUS
		Server.admin = clientSocket
		Server.adminIp = clientAddress
		details = None
		for index, client in enumerate(Server.allClients):
			if clientSocket in client:
				Server.allClients[index][2] = 1
				try:
					details = client[3]
				except:
					pass
		Server.adminAesKey = aesKey
		Log.ServerEventLog("ADMIN_LOGIN_SUCCESSFUL", details)
		aesEncryptor = AESCipher(aesKey)
		encryptedData = aesEncryptor.encrypt("INFRETSUCCESSFUL_ADMIN_LOGIN")
		clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		PrintSendToAdmin("SERVER **** ADMIN LOGGED IN            **** ADMIN(" + clientAddress + ")")
	
	# INITIALIZES ADMIN-PASSWORD CHANGE AND SEND OUT 2FA EMAIL
	def AdminPasswordChangeRequest(clientAddress, clientSocket, aesKey):
		# CHECK IF REQUEST ORIGINATES FROM ADMIN
		if not clientSocket == Server.admin:
			Handle.Error("PERM", None, clientAddress, clientSocket, aesKey, True)
			return
		# GENERATE VERIFICATION CODE
		codeFinal = CodeGenerator()
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		try:
			# UPDATE DATABASE AND SET THE NEW VERIFICATION CODE + ATTRIBUTES
			cursor.execute("UPDATE Tbl_user SET U_code = \"" + codeFinal + "\", U_codeTime = \"" + Timestamp() + "\", U_codeAttempts = 0, U_codeType = \"PASSWORD_CHANGE\" WHERE U_username = \"__ADMIN__\";")
		except Exception as e:
			connection.rollback()
			# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			connection.commit()
		finally:
			# FREE RESOURCES
			connection.close()
		# INITIALIZE VARIABLE TO STORE DETAILS IN
		details = None
		# GET DEVICE DETAILS
		allClients = Server.allClients
		for client in allClients:
			if clientSocket in client:
				details = client[3]
		if not details:
			# DETAILS HAVE NOT BEEN FOUND
			Handle.Error("NFND", "DETAILS_NOT_FOUND", clientAddress, clientSocket, aesKey, True)
			return
		# ADAPT FORMATTING TO WORK IN HTML
		htmlDetails = details.replace("\n","<br>")
		subject = "[PMDBS] Admin password change"
		text = "Hey Admin!\n\nYou have requested to change the admin password.\nThe request originated from the following device:\n\n" + details + "\n\nTo change your password, please enter the code below when prompted:\n\n" + codeFinal + "\n\nThe code is valid for 30 minutes.\nIf you did not request this email then there's someone out there playing around with admin privileges.\n*You should probably do something about that*\n\nBest regards,\nPMDBS Support Team"
		html = "<html><head><style>table.main {width:800px;background-color:#212121;color:#FFFFFF;margin:auto;border-collapse:collapse;}td.top {padding: 50px 50px 0px 50px;}td.header {background-color:#212121;color:#FF6031;padding: 0px 50px 0px 50px;}td.text {padding: 0px 50px 0px 50px;}td.bottom {padding: 0px 50px 50px 50px;}</style></head><body><table class=\"main\"><tr><td class=\"top\" align=\"center\"><img src=\"cid:icon1\" width=\"100\" height=\"100\"></td></tr><tr><td class=\"header\"><h3>Hey Admin!</h3></td></tr><tr><td class=\"text\"><p>You have requested to change the admin password.<br>The request originated from the following device:<br><br>" + htmlDetails + "<br><br>To change your password, please enter the code below when prompted:</p></td></tr><tr><td class=\"header\"><p align=\"center\"><b>" + codeFinal + "</b></p></td></tr><tr><td class=\"bottom\"><p><br>The code is valid for 30 minutes.<br>If you did not request this email then there's someone out there playing around with admin privileges.<br><b>*You should probably do something about that*</b><br><br>Best regards,<br>PMDBS Support Team</p></td></tr></table></body></html>"
		# CALL SENDMAIL
		Management.SendMail("PMDBS Support", SUPPORT_EMAIL_ADDRESS, subject, text, html, clientAddress)
		aesEncryptor = AESCipher(aesKey)
		encryptedData = aesEncryptor.encrypt("INFRETtodo%eq!SEND_VERIFICATION_ADMIN_CHANGE_PASSWORD!;")
		clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		
	# CHANGES THE ADMIN PASSWORD AFTER VALIDATING PROVIDED 2FA CODE
	def AdminPasswordChange(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE COMMAND
		# password%eq!password!;code%eq!code!;
		creds = command.split(";")
		# CHECK FOR SQL INJECTION ATTEMPTS
		if not DatabaseManagement.Security(creds, clientAddress, clientSocket, aesKey):
			return
		# SECURITY CHECK PASSED
		# CHECK IF REQUEST ORIGINATES FROM ADMIN
		if not clientSocket == Server.admin:
			Handle.Error("PERM", None, clientAddress, clientSocket, aesKey, True)
			return
		# INITAILIZE VARIABLES TO STORE EXTRACTED VALUES
		newPassword = None
		providedCode = None
		# EXTRACT VALUES FROM PROVIDED COMMAND
		try:
			for credential in creds:
				if "password" in credential:
					newPassword = credential.split("!")[1]
				elif "code" in credential:
					providedCode = credential.split("!")[1]
				elif len(credential) == 0:
					pass
				else:
					# COMMAND CONTAINS MORE DATA THAN REQUESTED --> THROW INVALID COMMAND EXCEPTION
					Handle.Error("ICMD", "TOO_MUCH_DATA", clientAddress, clientSocket, aesKey, True)
					return
		except Exception as e:
			# COMMAND HAS UNKNOWN FORMATTING --> THROW INVALID COMMAND EXCEPTION
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			return
		# VALIDATE THAT ALL VARIABLES HAVE BEEN INITIALIZED
		if not newPassword or not providedCode:
			Handle.Error("ICMD", "NOT_INITIALIZED", clientAddress, clientSocket, aesKey, True)
			return
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# INITALIZE VARIABLES TO STORE VALUES FOR CODE VALIDATION
		code = None
		codeTime = None
		codeType = None
		codeAttempts = None
		try:
			# QUERY DAATABASE FOR VALIDATION CODES
			cursor.execute("SELECT U_code, U_codeTime, U_codeType, U_codeAttempts FROM Tbl_user WHERE U_username = \"__ADMIN__\";")
			data = cursor.fetchall()
			code = data[0][0]
			codeTime = data[0][1]
			codeType = data[0][2]
			codeAttempts = data[0][3]
		except Exception as e:
			# SOMETHING SQL RELATED WENT WRONG / MIGHT ALSO BE INDEX OUT OF RANGE --> THROW EXCEPTION
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF ALL VALIABLES HAVE BEEN INITIALIZED
		if not code or not codeTime or codeAttempts == None or not codeType:
			Handle.Error("SQLE", "VARIABLES_NOT_INITIALIZED", clientAddress, clientSocket, aesKey, True)
			return
		if not codeType == "PASSWORD_CHANGE":
			Handle.Error("NCES", "NO_PASSWORD_CHANGE_SCHEDULED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF PASSWORD CHANGE HAS BEEN REQUESTED IN THE FIRST PLACE
		if codeAttempts == -1:
			Handle.Error("NCES", "NO_PASSWORD_CHANGE_SCHEDULED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF VALIDATION CODE MATCHES PROVIDED CODE
		if not code == providedCode:
			# CHECK IF NUMBER OF WRONG CODES HAS BEEN EXCEEDED
			if codeAttempts + 1 >= 3:
				# USER TRIES TO BRUTEFORCE VALIDATION CODE --> 1 HOUR BAN BY IP
				Handle.Error("F2FA", None, clientAddress, clientSocket, aesKey, True)
				# TODO: BAN DEVICE FOR 24H
				Management.Ban("ip%eq!" + clientAddress.split(":")[0] + "!;duration%eq!86400!;", clientAddress, clientSocket, aesKey, True)
				return
			else:
				# INCREMENT COUNTER FOR WRONG ATTEMPTS
				codeAttempts += 1
				try:
					# UPDATE COUNTER IN DATABASE
					cursor.execute("UPDATE Tbl_user Set U_codeAttempts = " + str(codeAttempts) + " WHERE U_username = \"__ADMIN__\";")
				except Exception as e:
					# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION AND FREE RESOURCES
					connection.rollback()
					connection.close()
					Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
					return
				else:
					# SUCCESSFULLY UPDATED DATABASE --> COMMIT CHANGES
					connection.commit()
					Handle.Error("I2FA", None, clientAddress, clientSocket, aesKey, True)
					return
		# CHECK IF VALIDATION CODE HAS EXPIRED
		if int(Timestamp()) - int(codeTime) > 1800:
			# CODE IS OLDER THAN 30 MINUTES AND THEREFORE INVALID
			Handle.Error("E2FA", None, clientAddress, clientSocket, aesKey, True)
			return
		# ALL CHECKS PASSED
		# UPDATE PASSWORD HASH
		hashedUsername = CryptoHelper.SHA256("__ADMIN__")
		salt = CryptoHelper.SHA256(hashedUsername + newPassword)
		newHash = CryptoHelper.Scrypt(newPassword, salt)
		try:
			# WRITE CHANGES TO DATABASE
			cursor.execute("UPDATE Tbl_user SET U_password = \"" + newHash + "\", U_codeAttempts = -1, U_lastPasswordChange = \"" + Timestamp() + "\", U_codeType = \"NONE\" WHERE U_username = \"__ADMIN__\";")
		except Exception as e:
			# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
			connection.rollback()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# UPDATED SUCCESSFULLY --> COMMIT CHANGES 
			connection.commit()
		finally:
			# FREE RESOURCES
			connection.close()
		# ALL UPDATED
		PrintSendToAdmin("SERVER ---> PASSWORD CHANGED           ---> " + clientAddress)
		aesEncryptor = AESCipher(aesKey)
		encryptedData = aesEncryptor.encrypt("INFRETSEND_UPDATE")
		clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
	
	# BAN A USER (ADMIN PRIVILEGES NEEDED)
	def BanAccount(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE COMMAND
		# username%eq!username!;duration%eq!duration_in_seconds!;
		# CHECK IF REQUEST ORIGINATES FROM ADMIN
		if not clientSocket == Server.admin:
			Handle.Error("PERM", None, clientAddress, clientSocket, aesKey, True)
			return
		parameters = command.split(";")
		# CHECK FOR SQL INJECTION
		if not DatabaseManagement.Security(parameters, clientAddress, clientSocket, aesKey):
			return
		# INITIALIZE VARIABLES
		username = None
		duration = None
		# STORE PROVIDED DATA IN VARIABLES
		try:
			for parameter in parameters:
				if "username" in parameter:
					username = parameter.split("!")[1]
				elif "duration" in parameter:
					duration = parameter.split("!")[1]
				elif len(parameter) == 0:
					pass
				else:
					# MORE DATA PROVIDED THAN NEEDED --> THROW EXCEPTION
					Handle.Error("ICMD", "TOO_MUCH_DATA", clientAddress, clientSocket, aesKey, True)
					return
		except Exception as e:
			# INVALID FORMATTING
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF CRUCIAL VARIABLES HAVE BEEN SET
		if not username or not duration:
			Handle.Error("ICMD", "TOO_FEW_PARAMETERS", clientAddress, clientSocket, aesKey, True)
			return
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		userExists = 0
		try:
			# QUERY DATABASE FOR USER
			cursor.execute("SELECT EXISTS(SELECT 1 FROM Tbl_user WHERE U_username = \"" + username + "\");")
			data = cursor.fetchall()
			userExists = data[0][0]
		except Exception as e:
			# SQL ERROR
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF USER EXISTS
		if not userExists == 1:
			Handle.Error("UDNE", None, clientAddress, clientSocket, aesKey, True)
			return
		try:
			# SET ISBANNED FLAG FOR USER
			cursor.execute("UPDATE Tbl_user SET U_isBanned = 1, U_banTime = \"" + Timestamp() + "\", U_banDuration = \"" + duration + "\" WHERE U_username = \"" + username + "\";")
		except Exception as e:
			# SQL ERROR --> ROLLBACK
			connection.rollback()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES
			connection.commit()
		finally:
			# FREE RESOURCES
			connection.close()
		PrintSendToAdmin("SERVER ---> BANNED ACCOUNT             ---> " + clientAddress)
		# KICK USER
		Management.Kick("mode%eq!username!;target%eq!" + username + ";!", clientAddress, clientSocket, aesKey)
	
	# ALLOWS CONNECTION TO SERVER AND SETS UP CLIENT HANDLER THREAD
	def AllowConnection(clientAddress, clientSocket):
		# DISPLAY IP OF CONNECTED CLIENT
		PrintSendToAdmin("SERVER ---> CONNECTED                  <--- " + clientAddress)
		# CREATE NEW THREAD FOR EACH CLIENT
		handlerThread = Thread(target = ClientHandler.Handler, args = (clientSocket, clientAddress))
		handlerThread.start()
		# ADD CLIENT DO CONNECTED CLIENTS
		Server.allClients.append([clientSocket, clientAddress, 0])
		logThread = Thread(target = Log.GetDetails, args = (clientAddress, clientSocket))
		logThread.start()
	
	# CHECKS IF CLIENT IS BANNED
	def SetupNewClient(clientAddress, clientSocket):
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		ip = clientAddress.split(":")[0]
		# CHECK FOR SQL INJECTION --> UNLIKELY BECAUSE IT'S CALLED LOCALLY
		if not DatabaseManagement.SecuritySilent([ip]):
			PrintSendToAdmin("SERVER <-#- [ERRNO 07]           SQLI  -#-> " + clientAddress)
			return
		# INITIALIZE VARIABLES
		time = None
		duration = None
		# GET VALUES FROM DATABASE AND STORE THEM IN VARIABLES
		try:
			cursor.execute("SELECT B_time, B_duration FROM Tbl_blacklist WHERE B_ip = \"" + ip + "\" ORDER BY B_id DESC;")
			data = cursor.fetchall()
			time = data[0][0]
			duration = data[0][1]
		except IndexError:
			# INDEX OUT OF RANGE --> CLIENT IS NOT BANNED
			Management.AllowConnection(clientAddress, clientSocket)
			return
		except sqlite3.Error:
			# SQL ERROR --> DISALLOW CONNECTION AND SEND STATUS TO ADMIN
			# TODO: LOG
			PrintSendToAdmin("SERVER ---> CONNECTION DENIED: SQLE    ---> " + clientAddress)
			clientSocket.send(b'\x01' + bytes("U" + "ERRCONNECTION_FAILED_INTERNAL_SERVER_ERROR", "utf-8") + b'\x04')
			return
		# CHECK IF ALL VARAIABLES HAVE BEEN SET
		if not time or not duration:
			PrintSendToAdmin("SERVER ---> CONNECTION DENIED: SQLE    ---> " + clientAddress)
			clientSocket.send(b'\x01' + bytes("U" + "ERRCONNECTION_FAILED_INTERNAL_SERVER_ERROR", "utf-8") + b'\x04')
			return
		# CHECK IF CLIENT IS ALLOWED TO CONNECT AGAIN
		if int(time) + int(duration) > int(Timestamp()):
			PrintSendToAdmin("SERVER ---> CONNECTION DENIED: BANNED  ---> " + clientAddress)
			clientSocket.send(b'\x01' + bytes("U" + "ERRCONNECTION_FAILED_BANNED", "utf-8") + b'\x04')
			return
		else:
			# ALL CHECKS PASSED --> ALLOW CONNECTION
			Management.AllowConnection(clientAddress, clientSocket)
	
	# BAN A CLIENT
	def Ban(command, clientAddress, clientSocket, aesKey, isSystem):
		# EXAMPLE COMMAND
		# ip%eq!ip!;duration%eq!duration_in_seconds!;
		# TODO: BAN ACCOUNT
		if not isSystem:
			# CHECK IF REQUEST COMES FROM ADMIN
			if not clientSocket == Server.admin:
				Handle.Error("PERM", None, clientAddress, clientSocket, aesKey, True)
				return
		parameters = command.split(";")
		# CHECK FOR SQL INJECTION
		if not DatabaseManagement.Security(parameters, clientAddress, clientSocket, aesKey):
			return
		# INITIALIZE VARIABLES
		ip = None
		duration = None
		# EXTRACT VALUES FROM PROVIDED COMMAND
		try:
			for parameter in parameters:
				if "ip" in parameter:
					ip = parameter.split("!")[1]
				elif "duration" in parameter:
					duration = parameter.split("!")[1]
				elif len(parameter) == 0:
					pass
				else:
					# COMMAND CONTAINS MORE DATA THAN REQUESTED --> THROW INVALID COMMAND EXCEPTION
					Handle.Error("ICMD", "TOO_MANY_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
					return
		except Exception as e:
			# COMMAND HAS UNKNOWN FORMATTING --> THROW INVALID COMMAND EXCEPTION
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF VARIABLES HAVE BEEN SET
		if not ip or not duration:
			# A CRUCIAL PARAMETER IS MISSING
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF IP IS VALID
		isValid = False
		try:
			isValid = [0<=int(x)<256 for x in re.split('\.',re.match(r'^\d+\.\d+\.\d+\.\d+$',ip).group(0))].count(True)==4
		except:
			# IP IS NOT VALID SEND ERROR MESSAGE
			Handle.Error("ICMD", "INVALID_IP_ADDRESS", clientAddress, clientSocket, aesKey, True)
			return
		if not isValid:
			# IP IS NOT VALID SEND ERROR MESSAGE
			Handle.Error("ICMD", "INVALID_IP_ADDRESS", clientAddress, clientSocket, aesKey, True)
			return
		# GET TIMESTAMP
		time = Timestamp()
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		try:
			# WRITE BAN RELATED INFORMATION TO DATABASE
			cursor.execute("INSERT INTO Tbl_blacklist (B_ip, B_time, B_duration) VALUES (\"" + ip + "\", \"" + time + "\", \"" + duration + "\")")
		except Exception as e:
			# ROLLBACK IN CASE OF ERROR
			connection.rollback()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES
			connection.commit()
		finally:
			# FREE RESOURCES
			connection.close()
		Management.Kick("mode%eq!ip!;target%eq!" + ip + ";!", clientAddress, clientSocket, aesKey)
		PrintSendToAdmin("SERVER ---> BANNED BY IP               ---> " + clientAddress)
		try:
			aesEncryptor = AESCipher(aesKey)
			encryptedData = aesEncryptor.encrypt("INFRETBANNED")
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		except:
			pass
		if not isSystem:
			PrintSendToAdmin("SERVER ---> ACKNOWLEDGE                ---> " + clientAddress)
	
	# DELETES AN ACCOUNT AFTER VALIDATING PROVIDED 2FA CODE
	def DeleteAccount(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE COMMAND
		# code%eq!code!;
		# GET USER ID
		userID = Management.CheckCredentials(clientAddress, clientSocket, aesKey)
		if not userID:
			# USER IS NOT LOGGED IN
			Handle.Error("NLGI", None, clientAddress, clientSocket, aesKey, True)
			return
		# EXAMPLE COMMAND
		# code%eq!code!;
		creds = command.split(";")
		# INITIALIZE VARIABLES
		providedCode = None
		# EXTRACT VALUES FROM PROVIDED COMMAND
		try:
			for credential in creds:
				if "code" in credential:
					providedCode = credential.split("!")[1]
				elif len(credential) == 0:
					pass
				else:
					# COMMAND CONTAINS MORE DATA THAN REQUESTED --> THROW INVALID COMMAND EXCEPTION
					Handle.Error("ICMD", "TOO_MANY_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
					return
		except Exception as e:
			# COMMAND HAS UNKNOWN FORMATTING --> THROW INVALID COMMAND EXCEPTION
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			return
		if not providedCode:
			# A CRUCIAL PARAMETER IS MISSING
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
			return
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# INITALIZE VARIABLES TO STORE VALUES FOR CODE VALIDATION
		code = None
		codeTime = None
		codeType = None
		codeAttempts = None
		try:
			# QUERY DAATABASE FOR VALIDATION CODES
			cursor.execute("SELECT U_code, U_codeTime, U_codeType, U_codeAttempts FROM Tbl_user WHERE U_id = " + userID + ";")
			data = cursor.fetchall()
			code = data[0][0]
			codeTime = data[0][1]
			codeType = data[0][2]
			codeAttempts = data[0][3]
		except Exception as e:
			# SOMETHING SQL RELATED WENT WRONG / MIGHT ALSO BE INDEX OUT OF RANGE --> THROW EXCEPTION
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF ALL VALIABLES HAVE BEEN INITIALIZED
		if not code or not codeTime or codeAttempts == None or not codeType:
			Handle.Error("SQLE", "VARIABLES_NOT_INITIALIZED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF NEW LOGIN HAS BEEN SCHEDULED
		if not codeType == "DELETE_ACCOUNT" or codeAttempts == -1:
			Handle.Error("NCES", "NO_NEW_LOGIN_SCHEDULED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF VALIDATION CODE MATCHES PROVIDED CODE
		if not code == providedCode:
			# CHECK IF NUMBER OF WRONG CODES HAS BEEN EXCEEDED
			if codeAttempts + 1 >= 3:
				# USER TRIES TO BRUTEFORCE VALIDATION CODE --> 1 HOUR BAN BY IP
				Handle.Error("F2FA", None, clientAddress, clientSocket, aesKey, True)
				# BAN DEVICE FOR 1H
				Management.Ban("ip%eq!" + clientAddress.split(":")[0] + "!;duration%eq!3600!;", clientAddress, clientSocket, aesKey, True)
				return
			else:
				# INCREMENT COUNTER FOR WRONG ATTEMPTS
				codeAttempts += 1
				try:
					# UPDATE COUNTER IN DATABASE
					cursor.execute("UPDATE Tbl_user Set U_codeAttempts = " + str(codeAttempts) + " WHERE U_username = " + username + ";")
				except Exception as e:
					# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION AND FREE RESOURCES
					connection.rollback()
					connection.close()
					Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
					return
				else:
					# SUCCESSFULLY UPDATED DATABASE --> COMMIT CHANGES
					connection.commit()
					Handle.Error("I2FA", None, clientAddress, clientSocket, aesKey, True)
					return
		# CHECK IF VALIDATION CODE HAS EXPIRED
		if int(Timestamp()) - int(codeTime) > 1800:
			# CODE IS OLDER THAN 30 MINUTES AND THEREFORE INVALID
			Handle.Error("E2FA", None, clientAddress, clientSocket, aesKey, True)
			return
		# ALL CODE CHECKS PASSED
		# LOGOUT USER
		Management.Logout(clientAddress, clientSocket, aesKey, False)
		try:
			# DELETE WHITELISTED COOKIES, USER DATA, USER LOG AND USER ACCOUNT
			cursor.execute("DELETE FROM Tbl_connectUserCookies WHERE U_id = " + userID + ";")
			cursor.execute("DELETE FROM Tbl_data WHERE D_userid = " + userID + ";")
			cursor.execute("DELETE FROM Tbl_clientLog WHERE L_userid = " + userID + ";")
			cursor.execute("DELETE FROM Tbl_user WHERE U_id = " + userID + ";")
		except Exception as e:
			# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION, ROLL BACK AND FREE RESOURCES
			connection.rollback()
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES
			connection.commit()
		finally:
			# FREE RESOURCES
			connection.close()
		# SEND CONFIRMATION TO CLIENT
		PrintSendToAdmin("SERVER ---> ACCOUNT DELETED            ---> " + clientAddress)
		aesEncryptor = AESCipher(aesKey)
		encryptedData = aesEncryptor.encrypt("INFRETACCOUNT_DELETED_SUCCESSFULLY")
		clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
	
	# WHITELISTS DEVICE COOKIE AFTER VALIDATING PROVIDED 2FA CODE
	def LoginNewDevice(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE COMMAND
		# username%eq!username!;code%eq!code!;password%eq!password!;cookie%eq!cookie!;
		creds = command.split(";")
		# CHECK FOR SQL INJECTION
		if not DatabaseManagement.Security(creds, clientAddress, clientSocket, aesKey):
			return
		# SECURITY CHECK PASSED
		# INITIALIZE VARIABLES
		username = None
		password = None
		cookie = None
		providedCode = None
		# EXTRACT VALUES FROM PROVIDED COMMAND
		try:
			for credential in creds:
				if "username" in credential:
					username = credential.split("!")[1]
				elif "password" in credential:
					password = credential.split("!")[1]
				elif "cookie" in credential:
					cookie = credential.split("!")[1]
				elif "code" in credential:
					providedCode = credential.split("!")[1]
				elif len(credential) == 0:
					pass
				else:
					# COMMAND CONTAINS MORE DATA THAN REQUESTED --> THROW INVALID COMMAND EXCEPTION
					Handle.Error("ICMD", "TOO_MANY_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
					return
		except Exception as e:
			# COMMAND HAS UNKNOWN FORMATTING --> THROW INVALID COMMAND EXCEPTION
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			return
		# VALIDATE THAT ALL VARIABLES HAVE BEEN INITIALIZED
		if not username or not providedCode or not password or not cookie:
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
			return
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# INITALIZE VARIABLES TO STORE VALUES FOR CODE VALIDATION
		code = None
		codeTime = None
		codeType = None
		codeAttempts = None
		isBanned = None
		banTime = None
		banDuration = None
		try:
			# QUERY DAATABASE FOR VALIDATION CODES
			cursor.execute("SELECT U_code, U_codeTime, U_codeType, U_codeAttempts, U_isBanned, U_banTime, U_banDuration FROM Tbl_user WHERE U_username = \"" + username + "\";")
			data = cursor.fetchall()
			code = data[0][0]
			codeTime = data[0][1]
			codeType = data[0][2]
			codeAttempts = data[0][3]
			isBanned = data[0][4]
			banTime = data[0][5]
			banDuration = data[0][6]
		except Exception as e:
			# SOMETHING SQL RELATED WENT WRONG / MIGHT ALSO BE INDEX OUT OF RANGE --> THROW EXCEPTION
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF ALL VALIABLES HAVE BEEN INITIALIZED
		if not code or not codeTime or codeAttempts == None or not username or not codeType or isBanned == None or not banTime or not banDuration:
			Handle.Error("SQLE", "VALIABLES_NOT_INITIALIZED", clientAddress, clientSocket, aesKey, True)
			return
		if isBanned == 1:
			if int(banTime) + int(banDuration) < int(Timestamp()):
				try:
					cursor.execute("UPDATE Tbl_user SET U_isBanned = 0 WHERE U_username = \"" + username + "\";")
				except Exception as e:
					connection.rollback()
					connection.close()
					Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
					return
				else:
					connection.commit()
			else:
				connection.close()
				Handle.Error("ACCB", None, clientAddress, clientSocket, aesKey, True)
				return
		# CHECK IF NEW LOGIN HAS BEEN SCHEDULED
		if not codeType == "NEW_LOGIN" or codeAttempts == -1:
			Handle.Error("NCES", "NO_NEW_LOGIN_SCHEDULED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF VALIDATION CODE MATCHES PROVIDED CODE
		if not code == providedCode:
			# CHECK IF NUMBER OF WRONG CODES HAS BEEN EXCEEDED
			if codeAttempts + 1 >= 3:
				# USER TRIES TO BRUTEFORCE VALIDATION CODE --> 1 HOUR BAN BY MAC ADDRESS OR IP
				Handle.Error("F2FA", None, clientAddress, clientSocket, aesKey, True)
				# BAN DEVICE FOR 1H
				Management.Ban("ip%eq!" + clientAddress.split(":")[0] + "!;duration%eq!3600!;", clientAddress, clientSocket, aesKey, True)
				return
			else:
				# INCREMENT COUNTER FOR WRONG ATTEMPTS
				codeAttempts += 1
				try:
					# UPDATE COUNTER IN DATABASE
					cursor.execute("UPDATE Tbl_user Set U_codeAttempts = " + str(codeAttempts) + " WHERE U_username = \"" + username + "\";")
				except Exception as e:
					# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION, ROLLBACK AND FREE RESOURCES
					connection.rollback()
					connection.close()
					Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
					return
				else:
					# SUCCESSFULLY UPDATED DATABASE --> COMMIT CHANGES
					connection.commit()
					Handle.Error("I2FA", None, clientAddress, clientSocket, aesKey, True)
					return
		# CHECK IF VALIDATION CODE HAS EXPIRED
		if int(Timestamp()) - int(codeTime) > 1800:
			# CODE IS OLDER THAN 30 MINUTES AND THEREFORE INVALID
			Handle.Error("E2FA", None, clientAddress, clientSocket, aesKey, True)
			return
		# CANCEL ACTIVE CODE
		try:
			cursor.execute("UPDATE Tbl_user SET U_codeAttempts = -1, U_codeType = \"NONE\";")
		# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
		except Exception as e:
			connection.rollback()
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# ACCOUNT SUCCESSFULLY VERIFIED --> COMMIT CHANGES
		else:
			connection.commit()
		# ALL CODE CHECKS PASSED
		# HASH PASSWORD
		hashedUsername = CryptoHelper.SHA256(username)
		salt = CryptoHelper.SHA256(hashedUsername + password)
		hashedPassword = CryptoHelper.Scrypt(password, salt)
		userID = None
		isVerified = None
		try:
			# CHECK IF CREDENTIALS ARE VALID
			# QUERY FOR USER ID
			cursor.execute("SELECT U_id, U_isVerified FROM Tbl_user WHERE U_username = \"" + username + "\" AND U_password = \"" + hashedPassword + "\";")
			# FETCH DATA
			data = cursor.fetchall()
			# GET USER ID
			userID = str(data[0][0])
			isVerified = data[0][1]
		except:
			connection.close()
			Log.ClientEventLog("LOGIN_ATTEMPT_FAILED", clientSocket)
			# RETURN ERROR MESSAGE TO CLIENT
			Handle.Error("CRED", None, clientAddress, clientSocket, aesKey, True)
			
			return
		try:
			# CREATE CONNECTION BETWEEN ACCOUNT ID AND COOKIE ID
			cursor.execute("INSERT INTO Tbl_connectUserCookies (U_id, C_id) VALUES (" + userID + ", (SELECT C_id FROM Tbl_cookies WHERE C_cookie = \"" + cookie + "\"));")
		except Exception as e:
			# SQL ERROR --> ROLLBACK
			connection.rollback()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES 
			connection.commit()
		finally:
			# FREE RESOURCES
			connection.close()
		# CREDENTIAL CHECK PASSED
		# CHECK IF USER ACCOUNT IS VERIFIED
		if isVerified == 0:
			PrintSendToAdmin("SERVER ---> ACCOUT NOT VERIFIED        ---> " + clientAddress)
			aesEncryptor = AESCipher(aesKey)
			encryptedData = aesEncryptor.encrypt("INFERRACCOUNT_NOT_VERIFIED")
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
			return
		# ADD USER TO WHITELIST
		Server.authorizedClients.append([userID, clientSocket, username])
		Log.ClientEventLog("LOGIN_SUCCESSFUL", clientSocket)
		aesEncryptor = AESCipher(aesKey)
		encryptedData = aesEncryptor.encrypt("INFLOGIN_SUCCESSFUL")
		clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		PrintSendToAdmin("SERVER ---> LOGIN SUCCESSFUL           ---> " + clientAddress)
		return
	
	# GENERATES A COOKIE AND SAVES IT TO THE DATABASE
	def RequestCookie(clientAddress, clientSocket, aesKey):
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR OBJECT
		cursor = connection.cursor()
		while True:
			# GET TIME AND CRYPTOGRAPHIC RANDOM
			currentTime = str(time.time())
			salt = str(secrets.randbelow(10**20))
			# USE BLAKE2 TO CREATE COOKIE
			cookie = CryptoHelper.BLAKE2(currentTime, salt)
			repeat = False
			try:
				# INSERT COOKIE INTO DATABASE
				cursor.execute("INSERT INTO Tbl_cookies (C_cookie) VALUES (\"" + cookie + "\")")
			except sqlite3.Error as e:
				# IF AN ERROR OCCURE DUE TO "UNIQUE" CONSTRAINT GENERATE NEW COOKIE
				if "UNIQUE" in str(e):
					repeat = True
				else:
					# AN UNKNOWN ERROR OCCURED
					# FREE RESOURCES
					connection.rollback()
					connection.close()
					Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
					return
			else:
				# COOKIE SUCCESSFULLY INSTERTED INTO DATABASE
				repeat = False
				# COMMIT CHANGES
				connection.commit()
			finally:
				# CHECK IF A NEW COOKIE HAS TO BE GENERATED
				if not repeat:
					# FREE RESOURCES AND BREAK OUT OF CURRENT SCOPE
					connection.close()
					break
		# RETURN COOKIE TO CLIENT
		PrintSendToAdmin("SERVER ---> COOKIE                     ---> " + clientAddress)
		aesEncryptor = AESCipher(aesKey)
		encryptedData = aesEncryptor.encrypt("DTACKIcookie%eq!" + cookie + "!;")
		clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
	
	# USES A CODE PROVIDED BY THE USER TO VERIFY THE EMAL ADDRESS
	def EmailVerification(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE COMMAND
		# username%eq!username!;code%eq!code!;
		creds = command.split(";")
		# CHECK FOR SQL INJECTION
		if not DatabaseManagement.Security(creds, clientAddress, clientSocket, aesKey):
			return
		# SECURITY CHECK PASSED
		# INITIALIZE VARIABLES
		username = None
		providedCode = None
		try:
			# POPULATE VARIABLES FROM COMMAND
			for credential in creds:
				if "username" in credential:
					username = credential.split("!")[1]
				elif "code" in credential:
					providedCode = credential.split("!")[1]
				elif len(credential) == 0:
					pass
				# COMMAND CONTAINS MORE DATA THAN REQUESTED --> THROW INVALID COMMAND ERROR
				else:
					Handle.Error("ICMD", "TOO_MANY_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
					return
		# COMMAND IS FORMATTED IN AN UNKNOWN WAY --> THROW INVALID COMMAND ERROR
		except Exception as e:
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			return
		# COMMAND DOES NOT CONTAIN ALL REQUIRED INFORMATION
		if not username or not providedCode:
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
			return
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# INITAILZE VERIABLES
		code = None
		codeTime = None
		codeType = None
		codeAttempts = None
		isVerified = None
		# EXECUTE SQL QUERY TO GET CODE RELATED DATA
		try:
			cursor.execute("SELECT U_isVerified, U_code, U_codeTime, U_codeType, U_codeAttempts from Tbl_user WHERE U_username = \"" + username + "\";")
			data = cursor.fetchall()
			isVerified = data[0][0]
			code = data[0][1]
			codeTime = data[0][2]
			codeType = data[0][3]
			codeAttempts = data[0][4]
		# THROW SQL ERROR / MAY ALSO BE INDEX OUT OF RANGE
		except Exception as e:
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF ALL VARIABLES ARE INITIALIZED
		if isVerified == None or not code or not codeTime or codeAttempts == None or not codeType:
			connection.close()
			Handle.Error("SQLE", "VARIABLES_NOT_INITIALIZED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF ACCOUNT ACTIVATION HAS BEEN SCHEDULED
		if not codeType == "ACTIVATE_ACCOUNT" or codeAttempts == -1:
			Handle.Error("NCES", "NO_PASSWORD_CHANGE_SCHEDULED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF CODE MATCHES THE CODE BY USER
		if not code == providedCode:
			# CHECK PROVIDED CODE WAS WRONG FOR THREE TIMES IN A ROW
			if codeAttempts + 1 >= 3:
				Handle.Error("F2FA", None, clientAddress, clientSocket, aesKey, True)
				# 2FA FAILED --> DELETE ACCOUNT
				try:
					cusor.execute("DELETE FROM Tbl_user WHERE U_id = " + userID + ";")
				except Exception as e:
					# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
					connection.rollback()
					Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
					return
				else:
					# ACCOUNT DELETED SUCCESSFULLY --> COMMIT CHANGES AND FREE RESSOURCES
					connection.commit()
				finally:
					# FREE RESOURCES
					connection.close()
				return
			# PROVIDED CODE WAS WRONG --> INCREMENT COUNTER
			else:
				codeAttempts += 1
				# UPDATE COUNTER IN DATABASE
				try:
					cursor.execute("UPDATE Tbl_user Set U_codeAttempts = " + str(codeAttempts) + " WHERE U_username = " + username + ";")
				# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
				except Exception as e:
					connection.rollback()
					connection.close()
					Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
					return
				# DATABASE UPDATED SUCCESSFULLY --> COMMIT CHANGES AND RETURN STATUS TO USER
				else:
					connection.commit()
					Handle.Error("I2FA", None, clientAddress, clientSocket, aesKey, True)
					return
		# CHECK IF CODE IS EXPIRED
		if int(Timestamp()) - int(codeTime) > 3600:
			Handle.Error("E2FA", None, clientAddress, clientSocket, aesKey, True)
			return
		# ALL CHECKS PASSED
		# VERIFY ACCOUNT
		try:
			cursor.execute("UPDATE Tbl_user SET U_isVerified = 1, U_codeAttempts = -1, U_lastPasswordChange = \"" + Timestamp() + "\", U_codeType = \"NONE\";")
		# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
		except Exception as e:
			connection.rollback()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# ACCOUNT SUCCESSFULLY VERIFIED --> COMMIT CHANGES
		else:
			connection.commit()
			PrintSendToAdmin("SERVER ---> ACCOUNT VERIFIED           ---> " + clientAddress)
			aesEncryptor = AESCipher(aesKey)
			encryptedData = aesEncryptor.encrypt("INFRETACCOUNT_VERIFIED")
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		# FREE RESOURCES
		finally:
			connection.close()
	
	# HANDLES REQUESTS FOR CHANGED MASTER PASSWORDS
	def MasterPasswordRequest(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE COMMAND
		# username%eq!username!;
		creds = command.split("!")
		# CHECK FOR SQL INJECTION ATTEMPTS
		if not DatabaseManagement.Security(creds, clientAddress, clientSocket, aesKey):
			return
		# SECURITY CHECK PASSED
		# INITIALIZE VARIABLE TO STORE THE USERNAME
		username = None
		# EXTRACT USERNAME FROM COMMAND
		try:
			for credential in creds:
				if "username" in credential:
					username = credential.split("!")[1]
				elif len(credential) == 0:
					pass
				else:
					# COMMAND CONTAINS MORE DATA THAN REQUESTED --> THROW INVALID COMMAND EXCEPTION
					Handle.Error("ICMD", "TOO_MANY_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
					return
		except Exception as e:
			# COMMAND HAS UNKNOWN FORMATTING --> THROW INVALID COMMAND EXCEPTION
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			return
		if not username:
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# INITIALIZE VARIABLE TO STORE PASSWORD-HASH IN
		passwordHash = None
		try:
			# QUERY DATABASE FOR PASSWORD HASH OF PROVIDED USERNAME
			cursor.execute("SELECT U_password from Tbl_user WHERE U_username = \"" + username + "\";")
			data = cursor.fetchall()
			passwordHash = data[0][0]
		except Exception as e:
			# SOMETHING SQL RELATED WENT WRONG / MIGHT ALSO BE INDEX OUT OF RANGE --> THROW EXCEPTION
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		finally:
			# FREE RESOURCES
			connection.close()
		# RETURN PASSSWORD HASH TO USER
		PrintSendToAdmin("SERVER ---> MASTERPASSWORD HASH        ---> " + clientAddress)
		aesEncryptor = AESCipher(aesKey)
		encryptedData = aesEncryptor.encrypt("DTARETSYNPWD" + passwordHash)
		clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
	
	# CHANGES THE PASSWORD AFTER VALIDATING PROVIDED 2FA CODE
	def PasswordChange(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE COMMAND
		# password%eq!passsword!;code%eq!code!;
		creds = command.split(";")
		# CHECK FOR SQL INJECTION ATTEMPTS
		if not DatabaseManagement.Security(creds, clientAddress, clientSocket, aesKey):
			return
		# SECURITY CHECK PASSED
		# CHECK CREDENTIALS
		userID = Management.CheckCredentials(clientAddress, clientSocket, aesKey)
		# CHECK IF USER IS LOGGED IN
		if not userID:
			# CHECK FAILED
			Handle.Error("NLGI", None, clientAddress, clientSocket, aesKey, True)
			return
		# INITAILIZE VARIABLES TO STORE EXTRACTED VALUES
		newPassword = None
		providedCode = None
		# EXTRACT VALUES FROM PROVIDED COMMAND
		try:
			for credential in creds:
				if "password" in credential:
					newPassword = credential.split("!")[1]
				elif "code" in credential:
					providedCode = credential.split("!")[1]
				elif len(credential) == 0:
					pass
				else:
					# COMMAND CONTAINS MORE DATA THAN REQUESTED --> THROW INVALID COMMAND EXCEPTION
					Handle.Error("ICMD", "TOO_MANY_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
					return
		except Exception as e:
			# COMMAND HAS UNKNOWN FORMATTING --> THROW INVALID COMMAND EXCEPTION
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			return
		# VALIDATE THAT ALL VARIABLES HAVE BEEN INITIALIZED
		if not newPassword or not providedCode:
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
			return
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# INITALIZE VARIABLES TO STORE VALUES FOR CODE VALIDATION
		code = None
		codeTime = None
		codeType = None
		codeAttempts = None
		username = None
		try:
			# QUERY DAATABASE FOR VALIDATION CODES
			cursor.execute("SELECT U_code, U_codeTime, U_codeType, U_codeAttempts, U_username FROM Tbl_user WHERE U_id = " + userID + ";")
			data = cursor.fetchall()
			code = data[0][0]
			codeTime = data[0][1]
			codeType = data[0][2]
			codeAttempts = data[0][3]
			username = data[0][4]
		except Exception as e:
			# SOMETHING SQL RELATED WENT WRONG / MIGHT ALSO BE INDEX OUT OF RANGE --> THROW EXCEPTION
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF ALL VALIABLES HAVE BEEN INITIALIZED
		if not code or not codeTime or codeAttempts == None or not username or not codeType:
			Handle.Error("SQLE", "VARIABLES_NOT_INITIALIZED", clientAddress, clientSocket, aesKey, True)
			return
		if not codeType == "PASSWORD_CHANGE":
			Handle.Error("NCES", "NO_PASSWORD_CHANGE_SCHEDULED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF PASSWORD CHANGE HAS BEEN REQUESTED IN THE FIRST PLACE
		if codeAttempts == -1:
			Handle.Error("NCES", "NO_PASSWORD_CHANGE_SCHEDULED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF VALIDATION CODE MATCHES PROVIDED CODE
		if not code == providedCode:
			# CHECK IF NUMBER OF WRONG CODES HAS BEEN EXCEEDED
			if codeAttempts + 1 >= 3:
				# USER TRIES TO BRUTEFORCE VALIDATION CODE --> 1 HOUR BAN BY IP
				Handle.Error("F2FA", None, clientAddress, clientSocket, aesKey, True)
				# TODO: BAN DEVICE FOR 1H
				Management.Ban("ip%eq!" + clientAddress.split(":")[0] + "!;duration%eq!3600!;", clientAddress, clientSocket, aesKey, True)
				return
			else:
				# INCREMENT COUNTER FOR WRONG ATTEMPTS
				codeAttempts += 1
				try:
					# UPDATE COUNTER IN DATABASE
					cursor.execute("UPDATE Tbl_user Set U_codeAttempts = " + str(codeAttempts) + " WHERE U_id = " + userID + ";")
				except Exception as e:
					# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION AND FREE RESOURCES
					connection.rollback()
					connection.close()
					Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
					return
				else:
					# SUCCESSFULLY UPDATED DATABASE --> COMMIT CHANGES
					connection.commit()
					Handle.Error("I2FA", None, clientAddress, clientSocket, aesKey, True)
					return
		# CHECK IF VALIDATION CODE HAS EXPIRED
		if int(Timestamp()) - int(codeTime) > 1800:
			# CODE IS OLDER THAN 30 MINUTES AND THEREFORE INVALID
			Handle.Error("E2FA", None, clientAddress, clientSocket, aesKey, True)
			return
		# ALL CHECKS PASSED
		# UPDATE PASSWORD HASH
		hashedUsername = CryptoHelper.SHA256(username)
		salt = CryptoHelper.SHA256(hashedUsername + newPassword)
		newHash = CryptoHelper.Scrypt(newPassword, salt)
		try:
			# WRITE CHANGES TO DATABASE
			cursor.execute("UPDATE Tbl_user SET U_password = \"" + newHash + "\", U_codeAttempts = -1, U_lastPasswordChange = \"" + Timestamp() + "\", U_codeType = \"NONE\" WHERE U_id = " + userID + ";")
		except Exception as e:
			# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
			connection.rollback()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# UPDATED SUCCESSFULLY --> COMMIT CHANGES 
			connection.commit()
		finally:
			# FREE RESOURCES
			connection.close()
		# ALL UPDATED
		# INITIALIZE SYNCRONIZATION (REQUEST UPDATED DATA FROM USER)
		PrintSendToAdmin("SERVER ---> PASSWORD CHANGED           ---> " + clientAddress)
		aesEncryptor = AESCipher(aesKey)
		encryptedData = aesEncryptor.encrypt("INFRETPASSWORD_CHANGED")
		clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')

	# HANDLES REQUESTS TO CHANGE THE MASTER PASSWORDS AND SENDS VERIFICATION CODES TO EMAIL
	def AccountRequest(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE COMMAND
		# mode%eq!PASSWORD_CHANGE!; OR mode%eq!DELETE_ACCOUNT!;
		# GET USER ID
		userID = Management.CheckCredentials(clientAddress, clientSocket, aesKey)
		if not userID:
			Handle.Error("NLGI", None, clientAddress, clientSocket, aesKey, True)
			return
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# INITIALIZE VARIABLES TO STORE DATABASE QUERY RESULTS
		address = None
		name = None
		try:
			# GET DATA NEEDED TO GENERATE EMAIL
			cursor.execute("SELECT U_email, U_name FROM Tbl_user WHERE U_id = " + userID + ";")
			data = cursor.fetchall()
			address = str(data[0][0])
			name = str(data[0][1])
		except Exception as e:
			# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF VARIABLES HAVE BEEN PROPERLY INITAILIZED
		if not address or not name:
			# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
			Handle.Error("SQLE", "VARIABLES_NOT_INITIALIZED", clientAddress, clientSocket, aesKey, True)
			return
		# GENERATE VERIFICATION CODE
		codeFinal = CodeGenerator()
		mode = None
		try:
			# EXAMPLE COMMAND
			# mode%eq!PASSWORD_CHANGE!;
			mode = command.split("!")[1]
		except Exception as e:
			# COMMAND WAS FORMATTED IN AN UNUSUAL MANNER
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			return
		if not mode == "PASSWORD_CHANGE" and not mode == "DELETE_ACCOUNT":
			# COMMAND IS INVALID
			Handle.Error("ICMD", "INVALID_MODE", clientAddress, clientSocket, aesKey, True)
			return
		try:
			# UPDATE DATABASE AND SET THE NEW VERIFICATION CODE + ATTRIBUTES
			timestamp = Timestamp()
			cursor.execute("UPDATE Tbl_user SET U_code = \"" + codeFinal + "\", U_codeTime = \"" + timestamp + "\", U_codeAttempts = 0, U_codeType = \"" + mode + "\" WHERE U_id = " + userID + ";")
			connection.commit()
		except Exception as e:
			connection.rollback()
			# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		finally:
			# FREE RESOURCES
			connection.close()
		# INITIALIZE VARIABLES TO STORE EMAIL RELATED INFORMATION
		subject = None
		text = None
		html = None
		details = None
		# GET DEVICE DETAILS
		allClients = Server.allClients
		for client in allClients:
			if clientSocket in client:
				details = client[3]
		if not details:
			# DETAILS HAVE NOT BEEN FOUND
			Handle.Error("NFND", "NO_DETAILS_FOUND", clientAddress, clientSocket, aesKey, True)
			return
		# ADAPT FORMATTING TO WORK IN HTML
		htmlDetails = details.replace("\n","<br>")
		if mode == "PASSWORD_CHANGE":
			# FILL NEEDED INFORMATION TO SEND EMAIL
			subject = "[PMDBS] Password change"
			text = "Dear " + name + "\n\nYou have requested to change your master password in our app.\nThe request originated from the following device:\n\n" + details + "\n\nTo change your password, please enter the code below when prompted:\n\n" + codeFinal + "\n\nThe code is valid for 30 minutes.\n\nBest regards,\nPMDBS Support Team"
			html = "<html><head><style>table.main {width:800px;background-color:#212121;color:#FFFFFF;margin:auto;border-collapse:collapse;}td.top {padding: 50px 50px 0px 50px;}td.header {background-color:#212121;color:#FF6031;padding: 0px 50px 0px 50px;}td.text {padding: 0px 50px 0px 50px;}td.bottom {padding: 0px 50px 50px 50px;}</style></head><body><table class=\"main\"><tr><td class=\"top\" align=\"center\"><img src=\"cid:icon1\" width=\"100\" height=\"100\"></td></tr><tr><td class=\"header\"><h3>Dear " + name + ",</h3></td></tr><tr><td class=\"text\"><p>You have requested to change your master password in our app. The request originated from the following device:<br><br>" + htmlDetails + "<br><br>To change your password, please enter the code below when prompted:</p></td></tr><tr><td class=\"header\"><p align=\"center\"><b>" + codeFinal + "</b></p></td></tr><tr><td class=\"bottom\"><p><br>The code is valid for 30 minutes.<br><br>Best regards,<br>PMDBS Support Team</p></td></tr></table></body></html>"
			# CALL SENDMAIL
			Management.SendMail("PMDBS Support", address, subject, text, html, clientAddress)
			aesEncryptor = AESCipher(aesKey)
			encryptedData = aesEncryptor.encrypt("INFRETtodo%eq!SEND_VERIFICATION_CHANGE_PASSWORD!;")
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		elif mode == "DELETE_ACCOUNT":
			# TODO ADAPT EMAIL TO "DELETE ACCOUNT"
			# FILL NEEDED INFORMATION TO SEND EMAIL
			subject = "[PMDBS] Delete your account?"
			text = "Dear " + name + ",\n\nYou have requested to delete your account and all data associated to it.\nThe request originated from the following device:\n\n" + details + "\n\nALL DATA WILL BE PERMANENTLY DELETED AND CANNOT BE RECOVERED!\nTo confirm your request, please enter the code below when prompted:\n\n" + codeFinal + "\n\nThe code is valid for 30 minutes.\n\nBest regards,\nPMDBS Support Team"
			html = "<html><head><style>table.main {width:800px;background-color:#212121;color:#FFFFFF;margin:auto;border-collapse:collapse;}td.top {padding: 50px 50px 0px 50px;}td.header {background-color:#212121;color:#FF6031;padding: 0px 50px 0px 50px;}td.text {padding: 0px 50px 0px 50px;}td.bottom {padding: 0px 50px 50px 50px;}</style></head><body><table class=\"main\"><tr><td class=\"top\" align=\"center\"><img src=\"cid:icon1\" width=\"100\" height=\"100\"></td></tr><tr><td class=\"header\"><h3>Dear " + name + ",</h3></td></tr><tr><td class=\"text\"><p>You have requested to delete your account and all data associated to it.<br>The request originated from the following device:<br><br>" + htmlDetails + "<br><br><b>ALL DATA WILL BE PERMANENTLY DELETED AND CANNOT BE RECOVERED!</b><br><br><br>To confirm your request, please enter the code below when prompted:</p></td></tr><tr><td class=\"header\"><p align=\"center\"><b>" + codeFinal + "</b></p></td></tr><tr><td class=\"bottom\"><p><br>The code is valid for 30 minutes.<br><br>Best regards,<br>PMDBS Support Team</p></td></tr></table></body></html>"
			# CALL SENDMAIL
			Management.SendMail("PMDBS Support", address, subject, text, html, clientAddress)
			aesEncryptor = AESCipher(aesKey)
			encryptedData = aesEncryptor.encrypt("INFRETtodo%eq!SEND_VERIFICATION_DELETE_ACCOUNT!;")
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		else:
			# COMMAND WAS INVALID
			Handle.Error("ICMD", "INVALID_MODE", clientAddress, clientSocket, aesKey, True)
			return
		
	
	# SENDS AN EMAIL USING GIVEN PARAMETERS
	def SendMail(From, To, subject, text, html, clientAddress):
		# CONNECT TO SMTP SERVER (SSL)
		server = smtplib.SMTP_SSL(host = SUPPORT_EMAIL_HOST, port = SUPPORT_EMAIL_SSL_PORT)
		# LOGIN
		server.login(SUPPORT_EMAIL_ADDRESS, SUPPORT_EMAIL_PASSWORD)
		message = MIMEMultipart("alternative")
		# SET EMAIL RELATED VARIABLES
		message["Subject"] = subject
		message["From"] = From
		message["To"] = To
		# READ IMAGE AS RAW BYTES
		imageFile = open("icon.png", "rb")
		msgImage = MIMEImage(imageFile.read())
		imageFile.close()
		# ADD HEADER TO IMAGE
		msgImage.add_header("Content-ID", "<icon1>")
		part1 = MIMEText(text, "plain")
		part2 = MIMEText(html, "html")
		# ATTACH IMAGE TO EMAIL
		message.attach(msgImage)
		# ATTACH TEXT TO EMAIL
		message.attach(part1)
		message.attach(part2)
		# SEND THE EMAIL
		server.sendmail(From, To, message.as_string())
		# DISCONNECT
		server.quit()
		PrintSendToAdmin("SERVER ---> VERIFICATION MAIL SENT     ---> " + clientAddress)
	
	# KICKS A CLIENT SPECIFIED BY THE COMMAND (CAN BE USERNAME, IP, IP:PORT)
	def Kick(command, clientAddress, clientSocket, aesKey):
		# CHECK IF REQUEST ORIGINATES FROM ADMIN
		if not clientSocket == Server.admin:
			Handle.Error("PERM", None, clientAddress, clientSocket, aesKey, True)
			return
		# mode%eq!ipport!;target%eq!192.168.178.50:52336!
		try:
			# GET MODE AND TARGET
			splittedCommands = command.split(";")
			mode = splittedCommands[0].split("!")[1]
			target = splittedCommands[1].split("!")[1]
			# CHECK FOR SUPPORTED MODES
			if mode == "ip":
				# TRY REGEX --> EXCEPTION == INVALID COMMAND
				try:
					[0<=int(x)<256 for x in re.split('\.',re.match(r'^\d+\.\d+\.\d+\.\d+$',target).group(0))].count(True)==4
				except Exception as e:
					Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
					return
				# COMMAND IS VALID
				else:
					kicked = False
					# CREATE LOCAL COPY OF THE SERVER PROPERTY "allClients"
					allClientsLocal = Server.allClients.copy()
					# ITERATE OVER LOCAL COPY OF ALL CLIENTS
					for client in allClientsLocal:
						# CHECK IF TARGET IP == CURRENT CLIENT
						if target in client[1]:
							# LOGOUT CLIENT (NOTE "NONE" FOR AESKEY IS ONLY VALID BECAUSE 4TH PARAMETER IS TRUE)
							Management.Logout(client[1], client[0], None, True)
							# DISCONNECT CLIENT
							Management.Disconnect(client[0], "KICKED_BY_ADMIN", client[1], False)
							kicked = True
					# CHECK IF CLIENT HAS BEEN KICKED
					if not kicked:
						# CLIENT NOT FOUND
						PrintSendToAdmin("SERVER ---> NO CLIENT FOUND            ---> " + clientAddress)
						aesEncryptor = AESCipher(aesKey)
						encryptedData = aesEncryptor.encrypt("INFCLIENT_NOT_FOUND")
						clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
					# CLIENT HAS BEEN KICKED
					else:
						# CKECK IF CLIENT WAS ADMIN
						if Server.admin == clientSocket:
							# SEND CONFIRMATION TO ADMIN
							PrintSendToAdmin("SERVER ---> ACKNOWLEDGE                ---> " + clientAddress)
							aesEncryptor = AESCipher(aesKey)
							encryptedData = aesEncryptor.encrypt("INFRETNOWLEDGE")
							clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
			elif mode == "ipport":
				ip = target.split(":")[0]
				port = target.split(":")[1]
				# TRY REGEX --> EXCEPTION == INVALID COMMAND
				try:
					[0<=int(x)<256 for x in re.split('\.',re.match(r'^\d+\.\d+\.\d+\.\d+$',ip).group(0))].count(True)==4 and re.match("^[0-9]{5}$",port) and int(port) < 65536
				except Exception as e:
					Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
					return
				# COMMAND IS VALID
				else:
					kicked = False
					# CREATE LOCAL COPY OF THE SERVER PROPERTY "allClients"
					allClientsLocal = Server.allClients.copy()
					# ITERATE OVER LOCAL COPY OF ALL CLIENTS
					for client in allClientsLocal:
						# CHECK IF TARGET IP == CURRENT CLIENT
						if target == client[1]:
							# LOGOUT CLIENT (NOTE "NONE" FOR AESKEY IS ONLY VALID BECAUSE 4TH PARAMETER IS TRUE)
							Management.Logout(client[1], client[0], None, True)
							# DISCONNECT CLIENT
							Management.Disconnect(client[0], "KICKED_BY_ADMIN", client[1], False)
							kicked = True
					# CHECK IF CLIENT HAS BEEN KICKED
					if not kicked:
						# CLIENT NOT FOUND
						PrintSendToAdmin("SERVER ---> NO CLIENT FOUND            ---> " + clientAddress)
						aesEncryptor = AESCipher(aesKey)
						encryptedData = aesEncryptor.encrypt("INFCLIENT_NOT_FOUND")
						clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
					# CLIENT HAS BEEN KICKED
					else:
						# CKECK IF CLIENT WAS ADMIN
						if Server.admin == clientSocket:
							# SEND CONFIRMATION TO ADMIN
							PrintSendToAdmin("SERVER ---> ACKNOWLEDGE                ---> " + clientAddress)
							aesEncryptor = AESCipher(aesKey)
							encryptedData = aesEncryptor.encrypt("INFRETNOWLEDGE")
							clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
			elif mode == "username":
				kicked = False
				# CREATE LOCAL COPY OF THE SERVER PROPERTY "authorizedClients"
				authorizedClientsLocal = Server.authorizedClients.copy()
				# ITERATE OVER LOCAL COPY OF AUTHORIZED CLIENTS
				for client in authorizedClientsLocal:
					# CHECK IF PROVIDED USERNAME MATCHES
					if client[2] == target:
						# GET ADDRESS FROM CLIENT LIST
						# ITERATE OVER CLIENT LIST
						for cclient in Server.allClients:
							# CHECK IF SOCKETS MATCH
							if client[1] in cclient:
								# LOGOUT CLIENT (NOTE "NONE" FOR AESKEY IS ONLY VALID BECAUSE 4TH PARAMETER IS TRUE)
								Management.Logout(client[1], client[0], None, True)
								# DISCONNECT CLIENT
								Management.Disconnect(cclient[0], "KICKED_BY_ADMIN", cclient[1], False)
								kicked = True
								break
				# CHECK IF CLIENT HAS BEEN KICKED
				if not kicked:
					# CLIENT NOT FOUND
					PrintSendToAdmin("SERVER ---> NO CLIENT FOUND            ---> " + clientAddress)
					aesEncryptor = AESCipher(aesKey)
					encryptedData = aesEncryptor.encrypt("INFCLIENT_NOT_FOUND")
					clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
				# CLIENT HAS BEEN KICKED
				else:
					# CKECK IF CLIENT WAS ADMIN
					if Server.admin == clientSocket:
						# SEND CONFIRMATION TO ADMIN
						PrintSendToAdmin("SERVER ---> ACKNOWLEDGE                ---> " + clientAddress)
						aesEncryptor = AESCipher(aesKey)
						encryptedData = aesEncryptor.encrypt("INFRETNOWLEDGE")
						clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
			# COMMAND IS INVALID
			else:
				Handle.Error("ICMD", "INVALID_MODE", clientAddress, clientSocket, aesKey, True)
		# SOMETHING WENT WRONG --> BETTER TELL ADMIN
		except Exception as e:
			# PRINT ERROR MESSAGE
			print("Error: {0}".format(e))
			try:
				Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			except:
				pass
	
	# REMOVES CLIENT FROM CLIENT LIST
	def Unlist(clientSocket):
		# ITERATE OVER CLIENT LIST
		for client in Server.allClients:
			# CHECK FOR MATCHING SOCKET
			if clientSocket in client:
				# REMOVE CLIENT
				Server.allClients.remove(client)
				return

	# DISCONNECT A CLIENT
	def Disconnect(clientSocket, message, address, ignoreErrors):
		# IF NO REASON IS SPECIFIED "UNKNOWN" WILL BE USED
		if message == "":
			message = "UNKNOWN"
		# IF CLIENT IS ADMIN FREE UP ADMIN SLOT
		if clientSocket == Server.admin:
			Server.admin = None
			Server.adminIp = None
			Log.ServerEventLog("ADMIN_LOGOUT", "IP: " + address)
			for client in Server.allClients:
				if clientSocket in client:
					Server.allClients.remove(client)
		# IF CLIENT IS USER REMOVE HIM FROM THE CURRENTLY-CONNECTED LIST
		else:
			for client in Server.allClients:
				if clientSocket in client:
					Server.allClients.remove(client)
		Log.ServerEventLog("CLIENT_DISCONNECTED", "IP: " + address)
		if not ignoreErrors:
			# SEND CUSTOM FIN
			clientSocket.send(b'\x01' + bytes("UFIN" + message, "utf-8") + b'\x04')
		# SEND TCP FIN
		clientSocket.shutdown(socket.SHUT_RDWR)
		# CLOSE SOCKET
		clientSocket.close()
		PrintSendToAdmin ("SERVER <-x- DISCONNECTED               -x-> " + address)

	# RETURNS LIST OF CLIENTS
	def ListClients(command, clientAddress, clientSocket, aesKey):
		# CHECK IF REQUEST ORIGINATES FROM ADMIN
		if not clientSocket == Server.admin:
			Handle.Error("PERM", None, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK FOR DIFFERENT FILTER MODES
		if command == "mode%eq!ALL_CONNECTED!;":
			# RETURN ALL CURRENTLY CONNECTED CLIENTS
			# CREATE A HEADER FOT THE TABLE
			header = CWHITE + FUNDERLINED + "IP:PORT" + 14 * " " + " │ STATUS" + 30 * " " + ENDF
			PrintSendToAdmin(header)
			# ITERATE OVER CLIENT LIST
			for client in Server.allClients:
				# GET IP AND SOCKET
				ip = client[1]
				csocket = client[0]
				# FIX PADDING TO ALIGN THE RESULTS
				ip += (21 - len(ip)) * " "
				clientStatus = None
				# CHECK IF CLIENT IS LOGGED IN
				# ITERATE OVER LIST OF AUTHORIZED CLIENTS
				for authClient in Server.authorizedClients:
					# CHECK FOR MATCH WITH SOCKET
					if csocket in authClient:
						# CONCATENATE RESULTS AND APPLY COLOR CODES
						clientStatus = CCYAN + ip + ENDF + CWHITE + " │ " + ENDF + CCYAN + "logged in as: " + authClient[2] + ENDF
						break
				# CHECK IF CLIENT IS ADMIN
				if not clientStatus:
					# ITERATE OVER CLIENT LIST
					for connClient in Server.allClients:
						# CHECK FOR ADMIN FLAG
						if csocket in connClient and connClient[2] == 1:
							# CONCATENATE RESULTS AND APPLY COLOR CODES
							clientStatus = CRED + ip + ENDF + CWHITE + " │ " + ENDF + CRED + "logged in as: ADMIN" + ENDF
							break
				# CLIENT IS NOT LOGGED IN
				if not clientStatus:
					# CONCATENATE RESULTS AND APPLY COLOR CODES
					clientStatus = CYELLOW + ip + ENDF + CWHITE + " │ " + ENDF + CYELLOW + "not logged in" + ENDF
				# PRINT IT
				PrintSendToAdmin(clientStatus)
		elif command == "mode%eq!ALL_USERS!;":
			# CREATE A HEADER FOT THE TABLE
			header = CWHITE + FUNDERLINED + "USERNAME" + 12 * " " + " │ STATUS" + 1 * " " + " │ LAST ONLINE (Zulu Time)" + ENDF
			PrintSendToAdmin(header)
			connection = sqlite3.connect(Server.dataBase)
			cursor = connection.cursor()
			allUsers = []
			try:
				try:
					cursor.execute("SELECT U_username from Tbl_user;")
					dataTable = cursor.fetchall()
					for row in dataTable:
						allUsers.append(row[0])
				except Exception as e:
					# SEND ERROR MESSAGE
					Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
					return
				for user in allUsers:
					status = "OFFLINE"
					lastSeen = "JUST NOW"
					for client in Server.authorizedClients:
						if user in client:
							status = "ONLINE"
					if Server.admin and user == "__ADMIN__":
						status = "ONLINE"
					if status == "OFFLINE":
						unixTime = None
						try:
							cursor.execute("SELECT L_datetime FROM Tbl_clientLog WHERE L_userid = (SELECT U_id from Tbl_user WHERE U_username = \"" + user + "\") AND L_event = \"LOGOUT\" ORDER BY L_id desc LIMIT 1;")
							data = cursor.fetchall()
							unixTime = data[0][0]
						except:
							pass
						if not unixTime == None:
							lastSeen = str(datetime.utcfromtimestamp(int(unixTime)).strftime("%Y-%m-%d %H:%M:%S"))
						else:
							lastSeen = "N/A"
					status += (7 - len(status)) * " "
					user += (20 - len(user)) * " "
					if "ONLINE" in status:
						status = CGREEN + status
					else:
						status = CRED + status
					if user.replace(" ","") == "__ADMIN__":
						user = CRED + user
						lastSeen = CRED + lastSeen
					else:
						user = CCYAN + user
						lastSeen = CCYAN + lastSeen
					entry = user + CWHITE + " │ " + status + CWHITE + " │ " + lastSeen + ENDF
					PrintSendToAdmin(entry)
			finally:
				connection.close()
		else:
			# COMMAND IS INVALID OR SELECTED MODE IS NOT SUPPORTED
			Handle.Error("ICMD", "INVALID_MODE", clientAddress, clientSocket, aesKey, True)
				
				
		
	# REGISTER A NEW USER 
	def Register(command, clientAddress, clientSocket, aesKey):
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# username%eq!username!;password%eq!password!;email%eq!email!;nickname%eq!nickname!;cookie%eq!cookie!;
		# SPLIT THE RAW COMMAND TO GET THE CREDENTIALS
		creds = command.split(";")
		# GET THE CREDENTIALS FROM ARRAY
		codeFinal = CodeGenerator()
		codeTime = Timestamp()
		# INITIALIZE VARIABLES TO STORE EXTRACTED INFORMATION FROM COMMAND
		username = None
		password = None
		email = None
		nickname = None
		cookie = None
		# ECTRACT INFORAMTION FROM COMMAND
		# TODO EXCEPTION HANDLING
		for credential in creds:
			if "username" in credential:
				username = credential.split("!")[1]
			elif "password" in credential:
				password = credential.split("!")[1]
			elif "email" in credential:
				email = credential.split("!")[1]
			elif "nickname" in credential:
				nickname = credential.split("!")[1]
			elif "cookie" in credential:
				cookie = credential.split("!")[1]
			elif len(credential) == 0:
				pass
			else:
				# COMMAND CONTAINED MORE DATA THAN REQUESTED
				Handle.Error("ICMD", "TOO_MANY_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
				return
		# VERIFY THAT ALL VARIABLES HAVE BEEN SET
		if not username or not password or not email or not nickname or not cookie:
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK FOR SQL INJECTION
		if not DatabaseManagement.Security(creds, clientAddress, clientSocket, aesKey):
			return
		# SQL INJECTION CHECK PASSED. CREDENTIALS ARE VALID
		# CREATE SCRYPT HASHED PASSWORD
		hashedUsername = CryptoHelper.SHA256(username)
		salt = CryptoHelper.SHA256(hashedUsername + password)
		hashedPassword = CryptoHelper.Scrypt(password, salt)
		try:
			# INSERT NEW USER INTO DATABASE
			cursor.execute("INSERT INTO Tbl_user (U_username,U_password,U_email,U_name,U_isVerified,U_code,U_codeTime,U_codeType,U_codeAttempts,U_lastPasswordChange,U_isBanned) VALUES (\"" + username + "\",\"" + hashedPassword + "\",\"" + email + "\",\"" + nickname + "\",0,\"" + codeFinal + "\",\"" + codeTime + "\",\"ACTIVATE_ACCOUNT\",0,\"" + Timestamp() + "\",0);")
		except Exception as e:
			# USER NAME ALREADY EXISTS
			connection.close()
			# SEND ERROR MESSAGE
			Handle.Error("UEXT", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES
			connection.commit()
			Log.ServerEventLog("REGISTER_NEW_USER", "User: " + username)
			# SEND ACKNOWLEDGEMENT TO CLIENT
			aesEncryptor = AESCipher(aesKey)
			# ENCRYPT DATA
			encryptedData = aesEncryptor.encrypt("INFRETREQUEST_VERIFICATION")
			PrintSendToAdmin("SERVER ---> ACKNOWLEDGE                ---> " + clientAddress)
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		isCookieValid = 0
		try:
			# CHECK IF THE PROVIDED COOKIE EXISTS
			cursor.execute("SELECT EXISTS(SELECT 1 FROM Tbl_cookies WHERE C_cookie = \"" + cookie + "\");")
			data = cursor.fetchall()
			isCookieValid = data[0][0]
		except Exception as e:
			# SQL ERROR
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		if isCookieValid == 0:
			# SEND ERROR MESSAGE
			Handle.Error("CDNE", None, clientAddress, clientSocket, aesKey, True)
			return
		try:
			# CREATE CONNECTION BETWEEN COOKIE AND USER IN DATABASE
			cursor.execute("INSERT INTO Tbl_connectUserCookies (U_id, C_id) SELECT U.U_id, C.C_id FROM Tbl_user as U, Tbl_cookies AS C WHERE U.U_username = \"" + username + "\" AND C.C_cookie = \"" + cookie + "\";")
		except Exception as e:
			# SQL ERROR --> ROLLBACK
			connection.rollback()
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES
			connection.commit()
		finally:
			# FREE RESOURCES
			connection.close()
		PrintSendToAdmin("SERVER ---> TODO: ACTIVATE ACCOUNT     ---> " + clientAddress)
		aesEncryptor = AESCipher(aesKey)
		encryptedData = aesEncryptor.encrypt("INFRETTODO_VERIFY_MAIL_SENT")
		clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		# GENERATE EMAIL
		subject = "[PMDBS] Please verify your email address."
		text = "Welcome, " + nickname + "!\n\nThe Password Management Database System enables you to securely store your passwords and confident information in one place and allows an easy access from all your devices.\n\nTo verify your account, please enter the following code when prompted:\n\n" + codeFinal + "\n\nThe code is valid for 30 minutes.\n\nBest regards,\nPMDBS Support Team"
		html = "<html><head><style>table.main {width:800px;background-color:#212121;color:#FFFFFF;margin:auto;border-collapse:collapse;}td.top {padding: 50px 50px 0px 50px;}td.header {background-color:#212121;color:#FF6031;padding: 0px 50px 0px 50px;}td.text {padding: 0px 50px 0px 50px;color:#FFFFFF;}td.bottom {padding: 0px 50px 50px 50px;}</style></head><body><table class=\"main\"><tr><td class=\"top\" align=\"center\"><img src=\"cid:icon1\" width=\"100\" height=\"100\"></td></tr><tr><td class=\"header\"><h2>Welcome, " + nickname + "!</h2></td></tr><tr><td class=\"text\"><p>The Password Management Database System enables you to securely store your passwords and confident information in one place and allows an easy access from all your devices.<br><br>To verify your account, please enter the following code when prompted:</p></td></tr><tr><td class=\"header\"><p align=\"center\"><b>" + codeFinal + "</b></p></td></tr><tr><td class=\"bottom\"><p><br><br>The code is valid for 30 minutes.<br><br>Best regards,<br>PMDBS Support Team</p></td></tr></table></body></html>"
		aesEncryptor = AESCipher(aesKey)
		encryptedData = aesEncryptor.encrypt("INFRETtodo%eq!SEND_VERIFICATION_ACTIVATE_ACCOUNT!;")
		clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		# SEND VERIFICATION CODE BY EMAIL
		Management.SendMail("PMDBS Support", email,	subject, text, html, clientAddress)
		
	# DUMP THE EVENT LOG / ADMIN PRIVILEGES REQUIRED
	def DumpEventLog(command, clientAddress, clientSocket, aesKey):
		# CHECK IF REQUEST ORIGINATES FROM ADMIN
		if not clientSocket == Server.admin:
			Handle.Error("PERM", None, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF PACKET IS VALID
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		try:
			# CHECK IF SERVER OR CLIENT LOG SHOULD BE DUMPED
			if command == "SERVER":
				# EXECUTE QUERY
				cursor.execute("SELECT * FROM Tbl_serverLog;")
				# FETCH DATA
				data = cursor.fetchall()
				# FORMAT DATA
				finalData = str(data).replace(", ",",").replace('[','').replace(']','')
				# ENCRYPT AND SEND TO ADMIN
				aesEncryptor = AESCipher(aesKey)
				encryptedData = aesEncryptor.encrypt("LOGDMPSERVER\n" + finalData)
				sendData = b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04'
				clientSocket.sendall(sendData)
				PrintSendToAdmin("SERVER ---> SERVER LOG DUMP            ---> " + clientAddress)
				# LOG DATA DUMP
				Log.ServerEventLog("SERVER_LOG_DUMP", "IP: " + clientAddress)
			elif command == "CLIENT":
				# EXECUTE QUERY
				cursor.execute("SELECT * FROM Tbl_clientLog;")
				# FETCH DATA
				data = cursor.fetchall()
				# FORMAT DATA
				finalData = str(data).replace(", ",",").replace('[','').replace(']','')
				# ENCRYPT AND SEND TO ADMIN
				aesEncryptor = AESCipher(aesKey)
				encryptedData = aesEncryptor.encrypt("LOGDMPCLIENT\n" + finalData)
				clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
				PrintSendToAdmin("SERVER ---> CLIENT LOG DUMP            ---> " + clientAddress)
				# LOG DATA DUMP
				Log.ServerEventLog("CLIENT_LOG_DUMP", "IP: " + clientAddress)
			else:
				return
		finally:
			# FREE RESOURCES
			connection.close()
		
	# ALLOWS TO LOG IN AS A REMOTE ADMIN
	def LoginAdmin(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE command = "password%eq!password!;cookie%eq!cookie!;"
		# SPLIT THE RAW COMMAND TO GET THE CREDENTIALS
		creds = command.split(";")
		# CHECK IF USER IS ADMIN
		if clientSocket == Server.admin:
			aesEncryptor = AESCipher(aesKey)
			encryptedData = aesEncryptor.encrypt("LOG|Ok! You are already Admin.")
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
			return
		# CHECK FOR SECURITY ISSUES
		if not DatabaseManagement.Security(creds, clientAddress, clientSocket, aesKey):
			return
		# INITIALIZE VARIABLES TO STORE CREDENTIALS IN
		password = None
		cookie = None
		# GET THE CREDENTIALS FROM ARRAY
		try:
			for credential in creds:
				if credential:
					if "password" in credential:
						password = credential.split("!")[1]
					elif "cookie" in credential:
						cookie = credential.split("!")[1]
					elif len(credential) == 0:
						pass
					else:
						# COMMAND CONTAINS MORE DATA THAN REQUESTED --> THROW INVALID COMMAND EXCEPTION
						Handle.Error("ICMD", "TOO_MANY_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
						return
		except Exception as e:
			# COMMAND CONTAINS UNKNOWN FORMATTING --> THROW INVALID COMMAND EXCEPTION
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			return
		# VALIDATE THAT ALL VARIABLES HAVE BEEN SET
		if not password or not cookie:
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
			return
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		cookieExists = 0
		isNewDevice = 1
		try:
			# CHECK IF COOKIE EXISTS AND CONNECTION BETWEEN ACCOUNT AND COOKIE IS EXISTENT
			cursor.execute("SELECT EXISTS(SELECT 1 FROM Tbl_cookies WHERE C_cookie = \"" + cookie + "\") UNION ALL SELECT NOT EXISTS(SELECT 1 FROM Tbl_user as U, Tbl_cookies as C, Tbl_connectUserCookies as CUC WHERE U.U_id = CUC.U_id and CUC.C_id = C.C_id and C.C_cookie = \"" + cookie + "\" and U.U_username = \"__ADMIN__\");")
			data = cursor.fetchall()
			cookieExists = data[0][0]
			isNewDevice = data[1][0]
		except Exception as e:
			# SQL ERROR 
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		if cookieExists == 0:
			# HANDLE INVALID COOKIES
			connection.close()
			Handle.Error("CDNE", None, clientAddress, clientSocket, aesKey, True)
			return
		# INITIALIZE VARIABLES TO STORE DETAILS IN
		details = None
		# GET DEVICE DETAILS
		allClients = Server.allClients
		for client in allClients:
			if clientSocket in client:
				details = client[3]
		if not details:
			# DETAILS HAVE NOT BEEN FOUND
			Handle.Error("NFND", "DETAILS_NOT_FOUND", clientAddress, clientSocket, aesKey, True)
			return
		if isNewDevice == 1:
			# CREATE CONNECTION TO DATABASE
			connection = sqlite3.connect(Server.dataBase)
			# CREATE CURSOR
			cursor = connection.cursor()
			# INITIALIZE VARIABLES TO STORE DATABASE QUERY RESULTS
			address = None
			name = None
			try:
				# GET DATA NEEDED TO GENERATE EMAIL
				cursor.execute("SELECT U_email, U_name FROM Tbl_user WHERE U_username = \"__ADMIN__\";")
				data = cursor.fetchall()
				address = str(data[0][0])
				name = str(data[0][1])
			except Exception as e:
				# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
				connection.close()
				Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
				return
			# CHECK IF VARIABLES HAVE BEEN PROPERLY SET
			if not address or not name:
				# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
				Handle.Error("SQLE", "VARIABLES_NOT_INITIALIZED", clientAddress, clientSocket, aesKey, True)
				return
			# GENERATE VERIFICATION CODE
			codeFinal = CodeGenerator()
			timestamp = Timestamp()
			try:
				# UPDATE DATABASE AND SET THE NEW VERIFICATION CODE + ATTRIBUTES
				cursor.execute("UPDATE Tbl_user SET U_code = \"" + codeFinal + "\", U_codeTime = \"" + timestamp + "\", U_codeType = \"NEW_LOGIN\", U_codeAttempts = 0 WHERE U_username = \"__ADMIN__\";")
			except Exception as e:
				# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION, ROLLBACK
				connection.rollback()
				Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
				return
			else:
				connection.commit()
			finally:
				# FREE RESOURCES
				connection.close()
			Handle.Error("DVFY", None, clientAddress, clientSocket, aesKey, True)
			# ADAPT FORMATTING TO WORK IN HTML
			htmlDetails = details.replace("\n","<br>")
			subject = "[PMDBS] Admin security warning"
			text = "Hey Admin!\n\nAre you trying to log in from a new device?\n\nSomeone just tried to log in as admin using the following device:\n\n" + details + "\n\nYou have received this email because we want to make sure that this is really you.\nTo verify that it is you, please enter the following code when prompted:\n\n" + codeFinal + "\n\nThe code is valid for 30 minutes.\n\nIf you did not try to sign in, you should consider changing the admin password.\n\nBest regards,\nPMDBS Support Team"
			html = "<html><head><style>table.main {width:800px;background-color:#212121;color:#FFFFFF;margin:auto;border-collapse:collapse;}td.top {padding: 50px 50px 0px 50px;}td.header {background-color:#212121;color:#FF6031;padding: 0px 50px 0px 50px;}td.text {padding: 0px 50px 0px 50px;color:#FFFFFF;}td.bottom {padding: 0px 50px 50px 50px;}</style></head><body><table class=\"main\"><tr><td class=\"top\" align=\"center\"><img src=\"cid:icon1\" width=\"100\" height=\"100\"></td></tr><tr><td class=\"header\"><h2>Hey Admin!</h2></td></tr><tr><td class=\"text\"><p><b>Are you trying to log in from a new device?</b><br><br>Someone just tried to log in as admin using the following device:<br><br>" + htmlDetails + "<br><br>You have received this email because we want to make sure that this is really you.<br>To verify that it is you, please enter the following code when prompted:</p></td></tr><tr><td class=\"header\"><p align=\"center\"><b>" + codeFinal + "</b></p></td></tr><tr><td class=\"bottom\"><p><br><br>The code is valid for 30 minutes.<br><br>If you did not try to sign in, you should consider <b>changing the admin password!</b><br><br>Best regards,<br>PMDBS Support Team</p></td></tr></table></body></html>"
			aesEncryptor = AESCipher(aesKey)
			encryptedData = aesEncryptor.encrypt("INFRETtodo%eq!SEND_VERIFICATION_ADMIN_NEW_DEVICE!;")
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
			# SEND ACCOUNT VALIDATION EMAIL
			Management.SendMail("PMDBS Support", SUPPORT_EMAIL_ADDRESS, subject, text, html, clientAddress)
			return
		# HASH PASSWORD
		hashedUsername = CryptoHelper.SHA256("__ADMIN__")
		salt = CryptoHelper.SHA256(hashedUsername + password)
		hashedPassword = CryptoHelper.Scrypt(password, salt)
		credentialsAreValid = 0
		try:
			# CHECK IF CREDENTIALS ARE VALID
			# QUERY FOR USER ID
			cursor.execute("SELECT EXISTS(SELECT 1 FROM Tbl_user WHERE U_username = \"__ADMIN__\" AND U_password = \"" + hashedPassword + "\");")
			# FETCH DATA
			data = cursor.fetchall()
			# GET USER ID
			credentialsAreValid = data[0][0]
		except Exception as e:
			# RETURN ERROR MESSAGE TO CLIENT 
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		finally:
			connection.close()
		if credentialsAreValid == 0:
			# THROW INVALID CREDENTIALS EXCEPTION
			# RETURN ERROR MESSAGE TO CLIENT
			Handle.Error("ADMN", None, clientAddress, clientSocket, aesKey, True)
			# CREATE LOG ENTRY
			Log.ServerEventLog("FAILED_ADMIN_LOGIN_ATTEMPT", details)
		# CREDENTIAL CHECK PASSED
		# CHECK FOR OTHER ADMINS
		if not Server.admin == None:
			# THROW ADMIN ALREADY LOGGED IN
			# RETURN ERROR MESSAGE TO CLIENT
			Handle.Error("ACNA", None, clientAddress, clientSocket, aesKey, True)
			return
		Management.Logout(clientAddress, clientSocket, aesKey, False)
		# SET UP ADMIN STATUS
		Server.admin = clientSocket
		Server.adminIp = clientAddress
		details = None
		for index, client in enumerate(Server.allClients):
			if clientSocket in client:
				Server.allClients[index][2] = 1
				try:
					details = client[3]
				except:
					pass
		Server.adminAesKey = aesKey
		Log.ServerEventLog("ADMIN_LOGIN_SUCCESSFUL", details)
		aesEncryptor = AESCipher(aesKey)
		encryptedData = aesEncryptor.encrypt("INFRETSUCCESSFUL_ADMIN_LOGIN")
		clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		PrintSendToAdmin("SERVER **** ADMIN LOGGED IN            **** ADMIN(" + clientAddress + ")")
			
	# DISCONNECT ALL CLIENTS AND SHUT DOWN SERVER
	def Shutdown(command, clientAddress, clientSocket, aesKey):
		# CHECK IF REQUEST ORIGINATES FROM ADMIN
		if not clientSocket == Server.admin:
			Handle.Error("PERM", None, clientAddress, clientSocket, aesKey, True)
			return
		PrintSendToAdmin(CWHITE + "         Initializing shutdown ..." + ENDF)
		# CHECK IF PACKET IS VALID
		if not command == "SHUTDOWN":
			PrintSendToAdmin(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Shutdown initalized." + ENDF)
			return
		# LOAD SERVER ATTRIBUTES TO LOCAL VARIABLES
		allClients = Server.allClients
		# CREATE LOG
		Log.ServerEventLog("SHUTDOWN", "Clients: " + str(allClients))
		# INITIALIZE SHUTDOWN SEQUENCE
		PrintSendToAdmin(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Shutdown initalized." + ENDF)
		PrintSendToAdmin(CWHITE + "         Closing sockets ..." + ENDF)
		Server.running = False
		time.sleep(REBOOT_TIME)
		PrintSendToAdmin(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Sockets closed." + ENDF)
		PrintSendToAdmin(CWHITE + "         Disconnecting clients ..." + ENDF)
		# INITIALIZE ITERATOR
		index = 0
		# ITERATE OVER CLIENT 2D ARRAY
		for client in allClients:
			# CHECK IF CURRENT CLIENT IS DEFAULT USER
			if client[2] == 0:
				# GET SOCKET + IP
				csocket = client[0]
				address = client[1]
				# DELETE CLIENT IN GLOBAL ARRAY
				del Server.allClients[index]
				# DISCONNECT CLIENT
				Management.Disconnect(csocket, "SERVER_SHUTDOWN", address, True)
			# CLIENT IS ADMIN
			else:
				# INCREMENT ITERATOR
				index += 1
		PrintSendToAdmin(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Disconnected clients." + ENDF)
		PrintSendToAdmin(CWHITE + "         Stopping threads ..." + ENDF)
		# WAIT 5 SECONDS IN CASE THREADS ARE STILL RUNNING
		time.sleep(REBOOT_TIME)
		PrintSendToAdmin(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Stopped threads." + ENDF)
		PrintSendToAdmin(CWHITE + "         Closing remote admin session ..." + ENDF)
		Server.stopped = True
		# CLOSE REMOTE SESSION
		Management.Disconnect(clientSocket, "SERVER_SHUTDOWN", clientAddress, True)
		time.sleep(1)
		PrintSendToAdmin(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Closed remote admin session." + ENDF)
		# CONNECT TO LOCALHOST TO TRIGGER socket.accept() (STUCK IN ENDLESS LOOP)
		PrintSendToAdmin(CWHITE + "         Triggering full stop ..." + ENDF)
		socket.create_connection((Server.localAddress, Server.localPort))
		PrintSendToAdmin(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Server shutdown complete." + ENDF)
		
		# DISCONNECT ALL CLIENTS AND REBOOT SERVER
	def Reboot(command, clientAddress, clientSocket, aesKey):
		# CHECK IF REQUEST COMES FROM ADMIN
		if not clientSocket == Server.admin:
			Handle.Error("PERM", None, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF PACKET IS VALID
		PrintSendToAdmin(CWHITE + "         Initializing reboot ..." + ENDF)
		if not command == "REBOOT":
			PrintSendToAdmin(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Reboot initalized." + ENDF)
			return
		# LOAD SERVER ATTRIBUTES TO LOCAL VARIABLES
		allClients = Server.allClients
		# CREATE LOG
		Log.ServerEventLog("REBOOT", "Clients: " + str(allClients))
		# INITIALIZE SHUTDOWN SEQUENCE
		PrintSendToAdmin(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Reboot initalized." + ENDF)
		PrintSendToAdmin(CWHITE + "         Closing sockets ..." + ENDF)
		Server.running = False
		time.sleep(REBOOT_TIME)
		PrintSendToAdmin(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Sockets closed." + ENDF)
		PrintSendToAdmin(CWHITE + "         Disconnecting clients ..." + ENDF)
		# INITIALIZE ITERATOR
		index = 0
		# ITERATE OVER CLIENT 2D ARRAY
		for client in allClients:
			# CHECK IF CURRENT CLIENT IS DEFAULT USER
			if client[2] == 0:
				# GET SOCKET + IP
				csocket = client[0]
				address = client[1]
				# DELETE CLIENT IN GLOBAL ARRAY
				del Server.allClients[index]
				# DISCONNECT CLIENT
				Management.Disconnect(csocket, "SERVER_REBOOT", address, True)
			# CLIENT IS ADMIN
			else:
				# INCREMENT ITERATOR
				index += 1
		PrintSendToAdmin(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Disconnected clients." + ENDF)
		PrintSendToAdmin(CWHITE + "         Stopping threads ..." + ENDF)
		# WAIT 5 SECONDS IN CASE THREADS ARE STILL RUNNING
		time.sleep(REBOOT_TIME)
		PrintSendToAdmin(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Stopped threads." + ENDF)
		PrintSendToAdmin(CWHITE + "         Closing remote admin session ..." + ENDF)
		Server.stopped = True
		# CLOSE REMOTE SESSION
		PrintSendToAdmin(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Closed remote admin session." + ENDF)
		time.sleep(1)
		Management.Disconnect(clientSocket, "SERVER_REBOOT", clientAddress, True)
		# CONNECT TO LOCALHOST TO TRIGGER socket.accept() (STUCK IN ENDLESS LOOP)
		print(CWHITE + "         Triggering full stop ..." + ENDF)
		socket.create_connection((Server.localAddress, Server.localPort))
		# RESTART SERVER
		print(CWHITE + "         Rescheduling process ..." + ENDF)
		print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Rescheduled process." + ENDF)
		print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Server shutdown complete." + ENDF)
		os.system("sleep " + str(REBOOT_TIME) + " && python3 dbserver.py")
	
	# LOGIN A CLIENT AND ADD HIM TO AUTHORIZED CLIENTS (SAVES EXPENSIVE DATABASE LOOKUPS)
	def Login(command, clientAddress, clientSocket, aesKey):
		# CHECK IF USER IS ADMIN
		if clientSocket == Server.admin:
			aesEncryptor = AESCipher(aesKey)
			encryptedData = aesEncryptor.encrypt("LOG|You are already Admin. Use \'logout\' and try again.")
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
			PrintSendToAdmin("SERVER ---> ALREADY LOGGED IN          ---> " + clientAddress)
			return
		# CHECK IF USER IS LOGGED IN ALREADY
		for client in Server.authorizedClients:
			if clientSocket in client:
				aesEncryptor = AESCipher(aesKey)
				encryptedData = aesEncryptor.encrypt("You are already logged in. Use \'logout\' and try again.")
				clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
				PrintSendToAdmin("SERVER ---> ALREADY LOGGED IN          ---> " + clientAddress)
				return
		# EXAMPLE command = "username%eq!username!;password%eq!password!;cookie%eq!cookie!;"
		# SPLIT THE RAW COMMAND TO GET THE CREDENTIALS
		creds = command.split(";")
		# CHECK FOR SECURITY ISSUES
		if not DatabaseManagement.Security(creds, clientAddress, clientSocket, aesKey):
			return
		# INITIALIZE VARIABLES TO STORE CREDENTIALS IN
		username = None
		password = None
		cookie = None
		# GET THE CREDENTIALS FROM ARRAY
		try:
			for credential in creds:
				if credential:
					if "username" in credential:
						username = credential.split("!")[1]
					elif "password" in credential:
						password = credential.split("!")[1]
					elif "cookie" in credential:
						cookie = credential.split("!")[1]
					elif len(credential) == 0:
						pass
					else:
						# COMMAND CONTAINS MORE DATA THAN REQUESTED --> THROW INVALID COMMAND EXCEPTION
						Handle.Error("ICMD", "TOO_MANY_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
						return
		except Exception as e:
			# COMMAND CONTAINS UNKNOWN FORMATTING --> THROW INVALID COMMAND EXCEPTION
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			return
		# VALIDATE THAT ALL VARIABLES HAVE BEEN SET
		if not username or not password or not cookie:
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
			return
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		userExists = 0
		cookieExists = 0
		isNewDevice = 1
		try:
			# CHECK IF COOKIE EXISTS AND CONNECTION BETWEEN ACCOUNT AND COOKIE IS EXISTENT
			cursor.execute("SELECT EXISTS(SELECT 1 FROM Tbl_cookies WHERE C_cookie = \"" + cookie + "\") UNION ALL SELECT NOT EXISTS(SELECT 1 FROM Tbl_user as U, Tbl_cookies as C, Tbl_connectUserCookies as CUC WHERE U.U_id = CUC.U_id and CUC.C_id = C.C_id and C.C_cookie = \"" + cookie + "\" and U.U_username = \"" + username + "\") UNION ALL SELECT EXISTS(SELECT 1 FROM Tbl_user WHERE U_username = \"" + username + "\");")
			data = cursor.fetchall()
			cookieExists = data[0][0]
			isNewDevice = data[1][0]
			userExists = data[2][0]
		except Exception as e:
			# SQL ERROR 
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		if userExists == 0:
			connection.close()
			Handle.Error("UDNE", None, clientAddress, clientSocket, aesKey, True)
			return
		if cookieExists == 0:
			# HANDLE INVALID COOKIES
			connection.close()
			Handle.Error("CDNE", None, clientAddress, clientSocket, aesKey, True)
			return
		isBanned = 1
		banTime = None
		banDuration = None
		try:
			cursor.execute("SELECT U_isBanned, U_banTime, U_banDuration FROM Tbl_user WHERE U_username = \"" + username + "\";")
			data = cursor.fetchall()
			isBanned = data[0][0]
			banTime = data[0][1]
			banDuration = data[0][2]
		except Exception as e:
			# SQL ERROR 
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		if isBanned == 1:
			if int(banTime) + int(banDuration) < int(Timestamp()):
				try:
					cursor.execute("UPDATE Tbl_user SET U_isBanned = 0 WHERE U_username = \"" + username + "\";")
				except Exception as e:
					connection.rollback()
					connection.close()
					Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
					return
				else:
					connection.commit()
			else:
				connection.close()
				Handle.Error("ACCB", None, clientAddress, clientSocket, aesKey, True)
				return
		if isNewDevice == 1:
			# CREATE CONNECTION TO DATABASE
			connection = sqlite3.connect(Server.dataBase)
			# CREATE CURSOR
			cursor = connection.cursor()
			# INITIALIZE VARIABLES TO STORE DATABASE QUERY RESULTS
			address = None
			name = None
			try:
				# GET DATA NEEDED TO GENERATE EMAIL
				cursor.execute("SELECT U_email, U_name FROM Tbl_user WHERE U_username = \"" + username + "\";")
				data = cursor.fetchall()
				address = str(data[0][0])
				name = str(data[0][1])
			except Exception as e:
				# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
				connection.close()
				Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
				return
			# CHECK IF VARIABLES HAVE BEEN PROPERLY SET
			if not address or not name:
				# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
				Handle.Error("SQLE", "VARIABLES_NOT_INITIALIZED", clientAddress, clientSocket, aesKey, True)
				return
			# GENERATE VERIFICATION CODE
			codeFinal = CodeGenerator()
			timestamp = Timestamp()
			try:
				# UPDATE DATABASE AND SET THE NEW VERIFICATION CODE + ATTRIBUTES
				cursor.execute("UPDATE Tbl_user SET U_code = \"" + codeFinal + "\", U_codeTime = \"" + timestamp + "\", U_codeType = \"NEW_LOGIN\", U_codeAttempts = 0 WHERE U_username = \"" + username + "\";")
			except Exception as e:
				# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION, ROLLBACK
				connection.rollback()
				Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
				return
			else:
				connection.commit()
			finally:
				# FREE RESOURCES
				connection.close()
			# INITIALIZE VARIABLES TO STORE EMAIL RELATED INFORMATION
			details = None
			# GET DEVICE DETAILS
			allClients = Server.allClients
			for client in allClients:
				if clientSocket in client:
					details = client[3]
			if not details:
				# DETAILS HAVE NOT BEEN FOUND
				Handle.Error("NFND", None, clientAddress, clientSocket, aesKey, True)
				return
			Handle.Error("DVFY", None, clientAddress, clientSocket, aesKey, True)
			# ADAPT FORMATTING TO WORK IN HTML
			htmlDetails = details.replace("\n","<br>")
			subject = "[PMDBS] Security warning"
			text = "Dear " + name + ",\n\nAre you trying to log in from a new device?\n\nSomeone just tried to log into your account using the following device:\n\n" + details + "\n\nYou have received this email because we want to make sure that this is really you.\nTo verify that it is you, please enter the following code when prompted:\n\n" + codeFinal + "\n\nThe code is valid for 30 minutes.\n\nIf you did not try to sign in, you should consider changing your master password. There is no need to panic though, your account is save as long as your email is not compromized as well.\n\nBest regards,\nPMDBS Support Team"
			html = "<html><head><style>table.main {width:800px;background-color:#212121;color:#FFFFFF;margin:auto;border-collapse:collapse;}td.top {padding: 50px 50px 0px 50px;}td.header {background-color:#212121;color:#FF6031;padding: 0px 50px 0px 50px;}td.text {padding: 0px 50px 0px 50px;color:#FFFFFF;}td.bottom {padding: 0px 50px 50px 50px;}</style></head><body><table class=\"main\"><tr><td class=\"top\" align=\"center\"><img src=\"cid:icon1\" width=\"100\" height=\"100\"></td></tr><tr><td class=\"header\"><h2>Dear " + name + ",</h2></td></tr><tr><td class=\"text\"><p><b>Are you trying to log in from a new device?</b><br><br>Someone just tried to log into your account using the following device:<br><br>" + htmlDetails + "<br><br>You have received this email because we want to make sure that this is really you.<br>To verify that it is you, please enter the following code when prompted:</p></td></tr><tr><td class=\"header\"><p align=\"center\"><b>" + codeFinal + "</b></p></td></tr><tr><td class=\"bottom\"><p><br><br>The code is valid for 30 minutes.<br><br>If you did not try to sign in, you should consider <b>changing your master password</b>. There is no need to panic though, your account is save as long as your email is not compromized as well.<br><br>Best regards,<br>PMDBS Support Team</p></td></tr></table></body></html>"
			aesEncryptor = AESCipher(aesKey)
			encryptedData = aesEncryptor.encrypt("INFRETtodo%eq!SEND_VERIFICATION_NEW_DEVICE!;")
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
			# SEND ACCOUNT VALIDATION EMAIL
			Management.SendMail("PMDBS Support", address, subject, text, html, clientAddress)
			return
		# CHECK IF USER ACCOUNT IS VERIFIED
		# HASH PASSWORD
		hashedUsername = CryptoHelper.SHA256(username)
		salt = CryptoHelper.SHA256(hashedUsername + password)
		hashedPassword = CryptoHelper.Scrypt(password, salt)
		userID = None
		isVerified = None
		try:
			# CHECK IF CREDENTIALS ARE VALID
			# QUERY FOR USER ID
			cursor.execute("SELECT U_id, U_isVerified FROM Tbl_user WHERE U_username = \"" + username + "\" AND U_password = \"" + hashedPassword + "\";")
			# FETCH DATA
			data = cursor.fetchall()
			# GET USER ID
			userID = str(data[0][0])
			isVerified = data[0][1]
		except:
			Log.ClientEventLog("LOGIN_ATTEMPT_FAILED", clientSocket)
			# RETURN ERROR MESSAGE TO CLIENT
			Handle.Error("CRED", None, clientAddress, clientSocket, aesKey, True)
			return
		finally:
			connection.close()
		# CREDENTIAL CHECK PASSED
		if isVerified == 0:
			PrintSendToAdmin("SERVER ---> ACCOUT NOT VERIFIED        ---> " + clientAddress)
			aesEncryptor = AESCipher(aesKey)
			encryptedData = aesEncryptor.encrypt("INFERRACCOUNT_NOT_VERIFIED")
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
			return
		# ADD USER TO WHITELIST
		Server.authorizedClients.append([userID, clientSocket, username])
		Log.ClientEventLog("LOGIN_SUCCESSFUL", clientSocket)
		aesEncryptor = AESCipher(aesKey)
		encryptedData = aesEncryptor.encrypt("INFRETLOGIN_SUCCESSFUL")
		clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		PrintSendToAdmin("SERVER ---> LOGIN SUCCESSFUL           ---> " + clientAddress)
	
	# REMOVES USER FROM WHITELIST
	def Logout(clientAddress, clientSocket, aesKey, isDisconnected):
		isLoggedout = False
		# CHECK IF USER IS ADMIN
		if clientSocket == Server.admin:
			# LOGOUT ADMIN
			# ITERATE OVER ALL CLIENTS
			for index, client in enumerate(Server.allClients):
				# CHECK IF ADMIN FLAG IS SET
				if client[2] == 1:
					# SET ADMIN FLAG TO 0
					Server.allClients[index][2] = 0
			Log.ServerEventLog("ADMIN_LOGOUT", "IP: " + clientAddress)
			Server.admin = None
			Server.adminIp = None
			isLoggedout = True
			print("SERVER **** ADMIN LOGOUT     **** " + clientAddress)
		else:
			# ITERATE OVER WHITELISTED CLIENTS AND REMOVE CLIENT TO LOGOUT
			for client in Server.authorizedClients:
				if clientSocket in client:
					if isDisconnected:
						Log.ClientEventLog("LOGOUT", clientSocket)
						Server.authorizedClients.remove(client)
						isLoggedout = True
					else:
						Log.ClientEventLog("LOGOUT", clientSocket)
						Server.authorizedClients.remove(client)
						isLoggedout = True
						PrintSendToAdmin("SERVER ---> LOGGED OUT                 ---> " + clientAddress)
						if not isDisconnected:
							aesEncryptor = AESCipher(aesKey)
							encryptedData = aesEncryptor.encrypt("INFRETLOGGED_OUT")
							clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
						break
			# USER IS NOT WHITELISTED
			if not isLoggedout and not isDisconnected:
				try:
					# RETURN ERROR MESSAGE TO CLIENT
					aesEncryptor = AESCipher(aesKey)
					encryptedData = aesEncryptor.encrypt("INFERRNOT_LOGGED_IN")
					clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
				except:
					clientSocket.send(b'\x01' + bytes("UINFERRNOT_LOGGED_IN", "utf-8") + b'\x04')
		# RETURN STATUS
		return isLoggedout
	
	# CHECKS IF USER IS WHITELISTED (SAVES EXPENSIVE DATABASE LOOKUPS)
	def CheckCredentials(clientAddress, clientSocket, aesKey):
		userID = None
		try:
			# ITERATE OVER 2D ARRAY
			for client in Server.authorizedClients:
				if clientSocket in client:
					userID = client[0]
		except:
			pass
		# USER IS NOT LOGGED IN
		if not userID:
			# RETURN ERROR MESSAGE TO CLIENT
			# ENCRYPT DATA
			aesEncryptor = AESCipher(aesKey)
			encryptedData = aesEncryptor.encrypt("INFERRNOT_LOGGED_IN")
			clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		# RETURN ID OR NONE IF USER NOT WHITELISTED
		return userID

# RETURNS A TIMESTAMP OF THE CURRENT DATETIME IN FORMAT YYYYMMDDHHMMSS		
def Timestamp():
	return str(time.time()).split('.')[0]
	
# REMOVES ALL CHARACTERS OF TEXT THAT ARE DEFINED IN CHARARRAY
def ReplaceAll(text, charArray):
	for char in charArray:
		text = text.replace(char,'')
	return text

# MIRRORS SERVER OUTPUT TO REMOTE ADMIN
def PrintSendToAdmin(text):
	print(text)
	# IF ADMIN IS LOGGED IN SEND SERVER MESSAGES
	if not Server.admin == None:
		aesEncryptor = AESCipher(Server.adminAesKey)
		encryptedData = aesEncryptor.encrypt("LOG|" + text)
		try:
			Server.admin.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
		except SocketError:
			Management.Logout(Server.adminIp, Server.admin, Server.adminAesKey, True)
def CodeGenerator():
	code = str(secrets.randbelow(1000000))
	while not len(code) >= 6:
		code = "0" + code
	return "PM-" + code
	
################################################################################
#-------------------------------SERVER MAIN THREAD-----------------------------#
################################################################################
# LISTENS ON PORT 4444 AND STARTS NEW THREAD FOR EACH CLIENT
class Server(Thread):
	# INITIALIZE GLOBAL SERVER ATTRIBUTES
	serverPublicKey = None
	serverPrivateKey = None
	publicKeyPem = None
	publicKeyXml = None
	geolocatingAvailable = True
	connection = None
	cursor = None
	dataBase = None
	localAddress = LOCAL_ADDRESS
	localPort = LOCAL_PORT
	allClients = [] #[[socket, address, adminFlag, details],[...]]
	authorizedClients = [] # [[ID, socket, username],[...]]
	admin = None
	adminIp = None
	running = True
	adminAesKey = None
	stopped = False
	TCPsocket = None
	nmap = False
	
	# RUN METHOD
	def run(self):
		# CLEAR THE CONSOLE
		os.system("cls" if os.name == "nt" else "clear")
		print(CWHITE + "         Initializing boot sequence ..." + ENDF)
		print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Boot sequence initialized." + ENDF)
		print(CWHITE + "         Checking python version ..." + ENDF)
		if not sys.version.split(" ")[0] == "3.6.6":
			print(CWHITE + "[" + CYELLOW + "WARNING" + CWHITE + "] Not tested on python " + sys.version.split(" ")[0] + ENDF)
		else:
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Current python version: 3.6.6" + ENDF)
		print(CWHITE + "         Checking config ..." + ENDF)
		if not REBOOT_TIME or not LOCAL_ADDRESS or LOCAL_ADDRESS == "" or not LOCAL_PORT or not SUPPORT_EMAIL_HOST or SUPPORT_EMAIL_HOST == "" or not SUPPORT_EMAIL_SSL_PORT or not SUPPORT_EMAIL_ADDRESS or SUPPORT_EMAIL_ADDRESS == "" or not SUPPORT_EMAIL_PASSWORD or SUPPORT_EMAIL_PASSWORD == "":
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] FATAL: Undefined variables in config file." + ENDF)
			return
		else:
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Checked config. " + ENDF)
		# GET ANY DATABASES IN CURRENT WORKING DIRECTORY
		print(CWHITE + "         Checking for database in " + os.getcwd() + " ..." + ENDF)
		dataBases = glob.glob(os.getcwd() + "/*.db")
		# EXIT IF NO DATABASES HAVE BEEN FOUND
		if len(dataBases) == 0:
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] FATAL: No database found." + ENDF)
			return
		# IF ONLY ONE DATABASE IS AVAILABLE AUTOSELECT IT
		elif len(dataBases) == 1:
			# GLOB RETURNS NAME + PATH --> REMOVE PATH
			pathParts = dataBases[0].split("/")
			db = pathParts[len(pathParts) - 1]
			# SET DATABASE
			Server.dataBase = db
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Found sqlite3 database \"" + self.dataBase + "\" in " + os.getcwd() + ENDF)
			print(CWHITE + "         Autoselecting ..." + ENDF)
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Selected " + self.dataBase + ENDF)
		# GLOB RETURNED MORE THAN ONE DATABASE --> MANUAL SELECTION NEEDED
		else:
			notSelected = True
			# LOOP IN CASE USER INPUT IS INVALID
			while notSelected:
				iterator = 0
				print(CWHITE + "         Found more than one databases in " + os.getcwd() + ENDF)
				print(CWHITE + "[" + CYELLOW + "MANUAL" + CWHITE + "] Which database do you want to use? (enter index)" + ENDF)
				cleanedDataBases = []
				# LIST AVAILABLE DATABASES
				for dbpath in dataBases:
					pathParts = dbpath.split("/")
					db = pathParts[len(pathParts) - 1]
					print(CWHITE + "         (" + str(iterator) + ") " + db + ENDF)
					cleanedDataBases.append(db)
					iterator = iterator + 1
				# PROMPT USER INPUT
				selectedDbString = input(CWHITE + " > " + ENDF)
				# TRY TO SELECT DATABASE AT SELECTED INDEX
				try:
					selectedDb = int(selectedDbString)
					Server.dataBase = cleanedDataBases[selectedDb]
					notSelected = False
				# INDEX WAS INVALID --> RETRY
				except:
					print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Invalid selection! Retrying ..." + ENDF)
					pass
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Selected " + self.dataBase + ENDF)
		# CHECK FOR READ / WRITE PERMISSIONS
		print(CWHITE + "         Checking permissions ..." + ENDF)
		print(CWHITE + "         Checking for READ permission ..." + ENDF)
		if os.access(self.dataBase, os.R_OK):
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Checked for READ permission." + ENDF)
		else:
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Checked for READ permission." + ENDF)
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] FATAL: Insufficient permissions!" + ENDF)
			return
		print(CWHITE + "         Checking for WRITE permission ..." + ENDF)
		if os.access(self.dataBase, os.W_OK):
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Checked for WRITE permission." + ENDF)
		else:
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Checked for WRITE permission." + ENDF)
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] FATAL: Insufficient permissions!" + ENDF)
			return
		print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Checked permissions." + ENDF)
		# CHECK FOR ADMIN ACCOUNT
		print(CWHITE + "         Verifying database integrity ..." + ENDF)
		connection = sqlite3.connect(self.dataBase)
		cursor = connection.cursor()
		userLen = None
		blacklistLen = None
		clientLogLen = None
		connectUserCookiesLen = None
		cookiesLen = None
		dataLen = None
		serverLogLen = None
		try:
			cursor.execute("PRAGMA table_info(Tbl_blacklist);")
			blacklistLen = len(cursor.fetchall())
			cursor.execute("PRAGMA table_info(Tbl_clientLog);")
			clientLogLen = len(cursor.fetchall())
			cursor.execute("PRAGMA table_info(Tbl_connectUserCookies);")
			connectUserCookiesLen = len(cursor.fetchall())
			cursor.execute("PRAGMA table_info(Tbl_cookies);")
			cookiesLen = len(cursor.fetchall())
			cursor.execute("PRAGMA table_info(Tbl_data);")
			dataLen = len(cursor.fetchall())
			cursor.execute("PRAGMA table_info(Tbl_serverLog);")
			serverLogLen = len(cursor.fetchall())
			cursor.execute("PRAGMA table_info(Tbl_user);")
			userLen = len(cursor.fetchall())
		except Exception as e:
			connection.close()
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Verifying database integrity: " + str(e) + ENDF)
			return
		if not userLen == TABLE_USER_LENGTH or not blacklistLen == TABLE_BLACKLIST_LENGTH or not clientLogLen == TABLE_CLIENTLOG_LENGTH or not connectUserCookiesLen == TABLE_CONNECTUSERCOOKIES_LENGTH or not cookiesLen == TABLE_COOKIES_LENGTH or not dataLen == TABLE_DATA_LENGTH or not serverLogLen == TABLE_SERVERLOG_LENGTH:
			connection.close()
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Verifying database integrity." + ENDF)
			return
		adminSet = 0
		try:
			cursor.execute("SELECT EXISTS(SELECT 1 FROM Tbl_user WHERE U_username = \"__ADMIN__\");")
			adminSet = cursor.fetchall()[0][0]
		except Exception as e:
			connection.close()
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Verifying database integrity: " + str(e) + ENDF)
			return
		if adminSet == 0:
			noMatch = True
			while noMatch:
				print(CWHITE + "         Admin password not set!" + ENDF)
				print(CWHITE + "[" + CYELLOW + "MANUAL" + CWHITE + "] Please enter the new admin password:" + ENDF)
				newAdminPassword = getpass.getpass(CWHITE + " > " + ENDF)
				print(CWHITE + "[" + CYELLOW + "MANUAL" + CWHITE + "] Please confirm the new admin password:" + ENDF)
				newAdminPasswordConfirm = getpass.getpass(CWHITE + " > " + ENDF)
				if newAdminPassword == newAdminPasswordConfirm and len(newAdminPassword) > 0:
					print(CWHITE + "         Setting admin password ..." + ENDF)
					adminPassword = CryptoHelper.SHA256(CryptoHelper.SHA256(newAdminPassword)[:32])
					hashedUsername = CryptoHelper.SHA256("__ADMIN__")
					salt = CryptoHelper.SHA256(hashedUsername + adminPassword)
					hashedPassword = CryptoHelper.Scrypt(adminPassword, salt)
					try:
						cursor.execute("INSERT INTO Tbl_user (U_username, U_name, U_password, U_email, U_isVerified, U_lastPasswordChange, U_isBanned) VALUES (\"__ADMIN__\", \"__ADMIN__\", \"" + hashedPassword + "\", \"" + SUPPORT_EMAIL_ADDRESS + "\", 1, \"" + Timestamp() + "\",0);")
					except Exception as e:
						connection.rollback()
						print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Setting admin password: " + str(e) + ENDF)
						return
					else:
						connection.commit()
						print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Set admin password." + ENDF)
						print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Verified database integrity." + ENDF)
						noMatch = False
					finally:
						connection.close()
				else:
					print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] These passwords don't match! Please try again." + ENDF)
		else:
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Verified database integrity." + ENDF)
		# LOG SERVER START
		print(CWHITE + "         Logging server start ..." + ENDF)
		Log.ServerEventLog("SERVER_STARTED", "IP: " + self.localAddress + "\nPort: " + str(self.localPort) + "\nBuild: " + NAME + " " + VERSION + " (" + BUILD + ", " + DATE + ", " + TIME + ") " + PYTHON)
		print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Logged server start." + ENDF)
		# IF NMAP IS INSTALLED AND HAS ROOT ACCESS ADVANCED LOGGING CAN BE USED
		print(CWHITE + "         Checking for Advanced Logging ..." + ENDF)
		print(CWHITE + "         Checking for nmap installation ..." + ENDF)
		nmapInstalled = os.system("type nmap >/dev/null 2>&1")
		if nmapInstalled == 0:
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] nmap is installed." + ENDF)
		else:
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] nmap is not installed / not accessible." + ENDF)
		print(CWHITE + "         Checking for root privileges ..." + ENDF)
		if os.geteuid() == 0:
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] root access." + ENDF)
		else:
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] root access." + ENDF)
		if nmapInstalled == 0 and os.geteuid() == 0:
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Enabled Advanced Logging." + ENDF)
			Server.nmap = True
		else:
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Enabled Advanced Logging." + ENDF)
		print(CWHITE + "         Checking for Geolocating ..." + ENDF)
		print(CWHITE + "         Checking for GeoDataBases in " + os.getcwd() + " ..." + ENDF)
		if os.path.isdir("GeoDataBases"):
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Directory \"GeoData\" exists." + ENDF)
			print(CWHITE + "         Checking for GeoLiteCity.dat in " + os.getcwd() + "/GeoDataBases ..." + ENDF)
			if os.path.isfile("GeoDataBases/GeoLiteCity.dat"):
				print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] File \"GeoLiteCity.dat\" exists." + ENDF)
			else:
				print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] File \"GeoLiteCity.dat\" not found." + ENDF)
				Server.geolocatingAvailable = False
			print(CWHITE + "         Checking for GeoIPASNum.dat in " + os.getcwd() + "/GeoDataBases ..." + ENDF)
			if os.path.isfile("GeoDataBases/GeoIPASNum.dat"):
				print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] File \"GeoIPASNum.dat\" exists." + ENDF)
			else:
				print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] File \"GeoIPASNum.dat\" not found." + ENDF)
				Server.geolocatingAvailable = False
		else:
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Directory \"GeoData\" not found." + ENDF)
			print(CWHITE + "         Skipping further checks ..." + ENDF)
			Server.geolocatingAvailable = False
		if Server.geolocatingAvailable:
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Enabled Geolocating." + ENDF)
		else:
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Enabled Geolocating." + ENDF)
		print(CWHITE + "         Generating RSA keys ..." + ENDF)
		try:
			# GENERATE RSA KEY PAIR
			keyPair = CryptoHelper.RSAKeyPairGenerator()
			Server.serverPublicKey = keyPair[0]
			Server.serverPrivateKey = keyPair[1]
			Server.publicKeyPem = Server.serverPublicKey.exportKey(format="PEM").decode("utf-8")
			Server.publicKeyXml = CryptoHelper.RSAPublicPemToXml(Server.publicKeyPem)
		except:
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Generated RSA keys." + ENDF)
			return
		print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Generated RSA keys." + ENDF)
		
		# CREATE SOCKET
		portBlocked = True
		while portBlocked:
			try:
				print(CWHITE + "         Setting up TCP listener on " + self.localAddress + ":" + str(self.localPort) + " ..." + ENDF)
				Server.TCPsocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
				# USE NON-BLOCKING SOCKET
				Server.TCPsocket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
				Server.TCPsocket.bind((self.localAddress, self.localPort))
				Server.TCPsocket.listen(1)
				portBlocked = False
			except Exception as error:
				print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Set up TCP listener on " + self.localAddress + ":" + str(self.localPort) + "." + ENDF)
				print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Error: {0}".format(error) + ENDF)
				print(CWHITE + "         Retrying in 5 seconds ..." + ENDF)
				time.sleep(5)
				Server.TCPsocket.close()
				pass
		print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Set up TCP listener on " + self.localAddress + ":" + str(self.localPort) + "." + ENDF)
		print("")
		print("                      _ _                       ")
		print("                     | | |                      ")
		print("  _ __  _ __ ___   __| | |__  ___   _ __  _   _ ")
		print(" | '_ \| '_ ` _ \ / _` | '_ \/ __| | '_ \| | | |")
		print(" | |_) | | | | | | (_| | |_) \__ \_| |_) | |_| |")
		print(" | .__/|_| |_| |_|\__,_|_.__/|___(_) .__/ \__, |")
		print(" | |                               | |     __/ |")
		print(" |_|                               |_|    |___/ ")
		print("------------------------------------------------")
		print(NAME + " " + VERSION + " (" + BUILD + ", " + DATE + ", " + TIME + ")")
		print("for " + PYTHON)
		print("Copyright (c) 2018 Frederik Hoeft")
		print("All Rights Reserved.")
		print("------------------------------------------------")
		print("")
		print("------------- AWAITING CONNECTIONS -------------")
		# CONTINUALLY ACCEPT INCOMING CONNECTIONS WHILE THE SERVER IS RUNNING
		try:
			while Server.running:
				clientSocket, address = Server.TCPsocket.accept()
				if Server.running:
					finalAddress = address[0] + ":" + str(address[1])
					# INITAILIZE CHECKS
					initThread = Thread(target = Management.SetupNewClient, args = (finalAddress, clientSocket))
					initThread.start()
		# CATCH SOCKET ERRORS
		except Exception as error:
			print("[-] FATAL: {0}".format(error))
		finally:
			# CLOSE PORT IN CASE OF ERROR
			Server.TCPsocket.close()

################################################################################
#--------------------------------CUSTOM PROTOCOL-------------------------------#
################################################################################
#-------------------------------PACKET SPECIFIER-------------------------------#
#
# U == UNENCRYPTED				--> PACKET IS UNENCRYPTED
# K == KEY EXCHANGE				--> PACKET IS RSA ENCRYPTED
# E == ENCRYPTED				--> PACKET IS AES ENCRYPTED
#
#----------------------------------PACKET IDS----------------------------------#
#
# PPK == "PEM PUBLIC KEY" 		--> PACKET CONTAINS RSA KEY IN PEM FORMAT
# XPK == "XML PUBLIC KEY" 		--> PACKET CONTAINS RSA KEY IN XML FORMAT
# EXC == "KEY EXCHANGE" 		--> PACKET CONTAINS AES KEY
# FIN == "FINISH CONNECTION"	--> CONNECTION IS BEING CLOSED
# DTA == "DATA"					--> PACKET CONTAINS AES ENCRYPTED DATA
# INF == "INFO"					--> PACKET CONTAINS INFORMATION ABOUT OTHER PACKETS
#
#--------------------------------PACKET SUB IDS--------------------------------#
#
# INF SUB IDS:
#	BGN == "BEGIN TRANSACTION"	--> CURRENTLY NOT IN USE
# 	END == "END TRANSACTION"	--> CURRENTLY NOT IN USE
# 	REG == "REGISTER USER"		--> REGISTER NEW USER
# 	CNG == "CHANGE CREDS"		--> CHANGE USER CREDENTIALS
# 	DEL == "DELETE ACCOUNT"		--> DELETE ACCOUNT OF USER
# 	ERR == "ERROR"				--> PACKET CONTAINS ERROR MESSAGE
# 	ACK == "ACKNOWLEDGE"		--> LAST COMMAND SUCCESSFULLY EXECUTED
#
# DTA SUB IDS:
#	RET == "RETURN"
#		INS == "INSERT"			--> CONTAINS ID OF INSERTED DATA
#		UPD == "UPDATE"			--> CONTAINS STATUS OF UPDATE QUERY
#		SEL == "SELECT"			--> CONTAINS SELECTED DATA
#
# REQ SUB IDS:
#	INS == "INSERT"				--> CONTAINS REQUEST TO INSERT DATA
#	UPD == "UPDATE"				--> CONTAINS REQUEST TO UPDATE DATA
#	SEL == "UPDATE"				--> CONTAINS REQUEST TO SELECT DATA
#
#---------------------------------TERMINATORS----------------------------------#
#
# \x01 == SOH					--> START OF HEADER (START OF TRANSMISSION)
# \x04 == EOT					--> END OF TRANSMISSION
# 
#---------------------------------ERROR CODES----------------------------------#
#
# [ERRNO 00] UNKN				--> UNKNOWN ERROR
# [ERRNO 01] IEOT				--> INVALID PACKET TERMINATOR
# [ERRNO 02] IRSA				--> INVALID RSA KEY
# [ERRNO 03] USEC				--> UNSECURE CONNECTION
# [ERRNO 04] IPID				--> INVALID PACKET ID
# [ERRNO 05] IPSP				--> INVALID PACKET SPECIFIER
# [ERRNO 06] ISID				--> INVALID PACKET SUB ID
# [ERRNO 07] SQLI				--> SQL INJECTION ATTEMPT
# [ERRNO 08] CRED				--> INVALID CREDENTIALS
# [ERRNO 09] ISQP				--> INVALID SQL QUERY PARAMETERS
# [ERRNO 10] ADMN				--> INVALID ADMIN CREDENTIALS
# [ERRNO 11] ACNA				--> ADMIN ALREADY LOGGED IN
# [ERRNO 12] PERM				--> INSUFFICIENT PERMISSIONS
# [ERRNO 13] NLGI				--> (LOGOUT ERROR) NOT LOGGED IN
# [ERRNO 14] ISOH				--> INVALID START OF HEADER
# [ERRNO 15] ICMD				--> INVALID COMMAND	
# [ERRNO 16] NFND				--> NOT FOUND
# [ERRNO 17] SQLE				--> SQL	ERROR
# [ERRNO 18] I2FA				--> INVALID 2FA CODE
# [ERRNO 19] E2FA				--> EXPIRED 2FA CODE
# [ERRNO 20] F2FA				--> FILED 2FA (3 TIMES WRONG CODE)
# [ERRNO 21] NCES				--> NO CODE EVENT SCHEDULED
# [ERRNO 22] UEXT				--> USER ALREADY EXISTS
# [ERRNO 23] CDNE				--> COOKIE DOES NOT EXIST
# [ERRNO 24] DVFY				--> VERIFY NEW DEVICE
#
#------------------------------------------------------------------------------#

################################################################################
#------------------------SERVER CONNECTION-HANDLER THREAD----------------------#
################################################################################
# MANAGES CONNECTION TO CLIENT
class ClientHandler():
	
	# MAIN FUNCTION
	def Handler(clientSocket, address):
		# TCP KEEP-ALIVE SETTINGS
		clientSocket.setsockopt(socket.SOL_SOCKET, socket.SO_KEEPALIVE, 1)
		# START SENDING KEEPALIVE PACKETS AFTER 10 SECONDS OF IDLING
		clientSocket.setsockopt(socket.SOL_TCP, socket.TCP_KEEPIDLE, 10)
		# SEND PACKETS IN 5 SECONDS INTERVAL
		clientSocket.setsockopt(socket.SOL_TCP, socket.TCP_KEEPINTVL, 5)
		# DROP CONNECTION AFTER 6 MISSED KEEPALIVE PACKETS (30 SECONDS WITHOUT CONNECTION)
		clientSocket.setsockopt(socket.SOL_TCP, socket.TCP_KEEPCNT, 6)
		# INITIALIZING VARIABLES
		clientPublicKey = None
		aesKey = None
		clientAddress = address
		isSocketError = False
		isDisconnected = False
		isTimedOut = False
		isTcpFin = False
		isXmlClient = False
		keyExchangeFinished = False
		# AWAIT PACKETS FROM CLIENT
		try:
			# BUFFER FOR HUGE PACKETS
			buf = b''
			# BREAK IF SERVER STOPPED
			while not Server.stopped and not clientSocket.fileno() == -1:
				receiving = True
				# RECEIVE AND DUMP TO BUFFER UNTIL EOT FLAG IS FOUND
				while receiving:
					# LOAD DATA TO 32768 BYTE BUFFER --> DOES NOT REALLY MATTER: MAX. ETHERNET PACKET SIZE IS 1500 BYTES
					data = clientSocket.recv(32768)
					# CHECK IF ANY DATA HAS BEEN RECEIVED
					if data:
						# INITIALIZE ARRAY FOR PACKETS
						dataPackets = None
						# ----HANDLE CASES OF MORE THAN ONE PACKET IN RECEIVE BUFFER----
						# CHECK IF PACKET CONTAINS EOT FLAG AND IF THE BUFFER IS EMPTY
						if b'\x04' in data and len(buf) == 0:
							# SPLIT PACKETS ON EOT FLAG (MIGHT BE MORE THAN ONE PACKET)
							rawDataPackets = data.split(b'\x04')
							# GRAB THE LAST PACKET
							lastDataPacket = rawDataPackets[len(rawDataPackets)-1]
							# MOVE ALL BUT THE LAST PACKET IN THE PACKET ARRAY
							dataPackets = rawDataPackets[0:len(rawDataPackets)-1]
							# IN CASE THE LAST PACKET CONTAINS DATA TOO MOVE IT IN BUFFER
							if not len(lastDataPacket) == 0:
								buf += lastDataPacket
							# SET RECEIVING TO FALSE TO BREAK THE LOOP
							receiving = False
						# CHECK IF PACKET CONTAINS DATA AND BUFFER IS NOT EMPTY
						elif b'\x04' in data and not len(buf) == 0:
							# SPLIT PACKETS ON EOT FLAG (MIGHT BE MORE THAN ONE PACKET)
							rawDataPackets = data.split(b'\x04')
							# APPEND BUFFER CONTENT TO FIRST PACKET
							rawDataPackets[0] = buf + rawDataPackets[0]
							# RESET THE BUFFER
							buf = b''
							# GRAB THE LAST PACKET
							lastDataPacket = rawDataPackets[len(rawDataPackets)-1]
							# MOVE ALL BUT THE LAST PACKET IN THE PACKET ARRAY
							dataPackets = rawDataPackets[0:len(rawDataPackets)-1]
							# IN CASE THE LAST PACKET CONTAINS DATA TOO MOVE IT IN BUFFER
							if not len(lastDataPacket) == 0:
								buf += lastDataPacket
							# SET RECEIVING TO FALSE TO BREAK THE LOOP
							receiving = False
						# THE PACKET DOES NOT CONTAIN ANY EOT FLAG
						else:
							# APPEND THE WHOLE RECEIVE BUFFER TO THE BUFFER AND REPEAT UNTIL EOT FLAG IS FOUND
							buf += data
					else:
						isTcpFin = True
						return
				# ITERATE OVER PACKET ARRAY
				for dataPacket in dataPackets:
					# ADMIN IS ALLOWED TO STAY CONNECTED IN SHUTDOWN SEQUENCE
					if Server.running or Server.admin == clientSocket:
						# CHECK FOR ADMIN PRIVILEGES (AND DISPLAY FANCY "ADMIN" BADGE)
						if Server.admin == clientSocket and not "ADMIN" in clientAddress:
							clientAddress = "ADMIN(" + clientAddress + ")"
						elif not Server.admin == clientSocket and "ADMIN" in clientAddress:
							clientAddress = address
						# ----PACKET VALIDATION----
						# CHECK IF SOH FLAG IS *NOT* FIRST CHARACTER
						if not dataPacket[0] == b'\x01':
							# IF THE PACKET DOES NOT CONTAIN A SOH FLAG PACKET IS INVALID CONINUE WITH NEXT PACKET
							if dataPacket.count(b'\x01') == 0:
								# PACKET IS INVALID
								PrintSendToAdmin("SERVER <-#- [ERRNO 14] ISOH            -#-> " + clientAddress)
								continue
							# IF THE PACKET CONTAINS A SOH FLAG BUT IT IS NOT IN THE BEGINNING REMOVE THE BEGINNING UNTIL SOH IS FIRST CHARACTER
							elif dataPacket.count(b'\x01') == 1:
								dataPacket = b'\x01' + dataPacket.split(b'\x01')[1]
							# THE PACKET CONTAINS MORE THAN ONE SOH. SOMETHING WENT HORRIBLY WRONG
							else:
								PrintSendToAdmin("SERVER <-#- [ERRNO 14] ISOH            -#-> " + clientAddress)
								continue
						# DECODE THE PACKET TO UTF-8 STRING
						dataString = dataPacket[1:].decode("utf-8")
						# GET PACKET SPECIFIER
						packetSpecifier = dataString[:1]
						# CHECK IF DATA IS UNENCRYPTED
						if packetSpecifier == 'U':
							# GET PACKETID
							packetID = dataString[1:4]
							# CHECK IF PACKET IS DEAUTH PACKET
							if packetID == "FIN":
								isDisconnected = True
								# JUMP TO FINALLY AND FINISH CONNECTION
								return
							elif packetID == "INI":
								PrintSendToAdmin("SERVER <--- CLIENT HELLO               <--- " + clientAddress)
								# GET PACKET ID
								packetSID = dataString[4:7]
								# GET FORMAT
								if packetSID == "XML":
									isXmlClient = True
									# SEND PUBLIC KEY
									clientSocket.send(b'\x01' + bytes("UKEYXMLkey%eq!" + Server.publicKeyXml + "!;", "utf-8") + b'\x04')
								elif packetSID == "PEM":
									isXmlClient = False
									# SEND PUBLIC KEY
									clientSocket.send(b'\x01' + bytes("UKEYPEMkey%eq!" + Server.publicKeyPem + "!;", "utf-8") + b'\x04')
								else:
									PrintSendToAdmin("SERVER <-#- [ERRNO 02] IRSA            -#-> " + clientAddress)
									return
								PrintSendToAdmin("SERVER ---> SERVER HELLO               ---> " + clientAddress)
							# CHECK IF PACKET ID IS KNOWN BUT USED IN WRONG CONTEXT
							elif packetID in ("EXC", "DTA", "INF", "REQ", "MNG", "LOG", "KEX"):
								# RECEIVED SOME OTHER PACKET OVER UNENCRYPTED CONNECTION
								# UNSECURE CONNECTION
								PrintSendToAdmin("SERVER <-#- [ERRNO 03] USEC             -#-> " + clientAddress)
								# JUMP TO FINALLY AND FINISH CONNECTION
								return
							else:
								# RECEIVED INVALID PACKET ID
								PrintSendToAdmin("SERVER <-#- [ERRNO 04] IPID            -#-> " + clientAddress)
								# JUMP TO FINALLY AND FINISH CONNECTION
								return
						# CHECK IF PACKET "IS KEY EXCHANGE" (RSA ENCRYPTED) 
						elif packetSpecifier == 'K':
							# GET PACKETID
							packetID = dataString[1:4]
							if packetID == "CKE":
								PrintSendToAdmin("SERVER <--- CLIENT KEY EXCHANGE        <--- " + clientAddress)
								command = dataString[4:]
								# EXAMPLE COMMAND = key%eq!key!;nonce%eq!encrypted_nonce!;
								cryptoInformation = command.split(";")
								key = None
								nonce = None
								# EXTRACT KEY AND NONCE FROM PACKET
								# TODO: RETURN ERRORS TO CLIENT
								try:
									for info in cryptoInformation:
										if "key" in info:
											key = info.split("!")[1]
										elif "nonce" in info:
											nonce = info.split("!")[1]
										elif len(info) == 0:
											pass
										else:
											# COMMAND CONTAINED MORE INFORMATION THAN REQUESTED
											PrintSendToAdmin("SERVER <-#- [ERRNO 15] ICMD            -#-> " + clientAddress)
											return
								except:
									# COMMAND HAS UNKNOWN FORMATTING
									PrintSendToAdmin("SERVER <-#- [ERRNO 15] ICMD            -#-> " + clientAddress)
									return
								# COMMAND DID NOT CONTAIN ALL INFORMATION
								if not nonce or not key:
									PrintSendToAdmin("SERVER <-#- [ERRNO 15] ICMD            -#-> " + clientAddress)
									return
								try:
									if isXmlClient:
										clientPublicKey = CryptoHelper.RSAPublicXmlToPem(key)
									else:
										clientPublicKey = key
								except:
									PrintSendToAdmin("SERVER <-#- [ERRNO 02] IRSA            -#-> " + clientAddress)
									return
								# GENERATE 256 BIT AES KEY
								aesKey = CryptoHelper.AESKeyGenerator()
								# PrintSendToAdmin("AES: " + aesKey)
								# ENCRYPT AES KEY USING RSA 4096
								decNonce = CryptoHelper.RSADecrypt(nonce, Server.serverPrivateKey)
								message = "SKEkey%eq!" + aesKey + "!;nonce%eq!" + decNonce + "!;"
								encryptedMessage = CryptoHelper.RSAEncrypt(message, clientPublicKey)
								# CONVERT KEY TO BYTES
								finalBytes = b'\x01' + bytes("K" + encryptedMessage, "utf-8") + b'\x04'
								# SEND KEY TO CLIENT
								clientSocket.send(finalBytes)
								PrintSendToAdmin("SERVER ---> SYMMETRIC KEY EXCHANGE     ---> " + clientAddress)
							else:
								# RECEIVED INVALID PACKET ID
								PrintSendToAdmin("SERVER <-#- [ERRNO 04] IPID            -#-> " + clientAddress)
								# JUMP TO FINALLY AND FINISH CONNECTION
								return
						# CHECK IF PACKET IS AES ENCRYPTED
						elif packetSpecifier == 'E':
							# CREATE AES CIPHER
							aesDecryptor = AESCipher(aesKey)
							# DECRYPT DATA
							decryptedData = aesDecryptor.decrypt(dataString[1:])
							# DEBUG PrintSendToAdmin DATA
							# GET PACKET ID
							packetID = decryptedData[:3]
							# GET PACKET SUB-ID
							packetSID = decryptedData[3:6]
							if packetID == "KEX":
								if packetSID == "ACK":
									command = decryptedData[6:]
									nonce = None
									if "nonce" in command:
										nonce = command.split("!")[1]
									else:
										PrintSendToAdmin("SERVER <-#- [ERRNO 15] ICMD            -#-> " + clientAddress)
										return
									# ENCRYPT DATA
									message = "nonce%eq!" + nonce + "!;"
									aesEncryptor = AESCipher(aesKey)
									encryptedData = aesEncryptor.encrypt("KEXACK" + message)
									clientSocket.send(b'\x01' + bytes("E" + encryptedData, "utf-8") + b'\x04')
									PrintSendToAdmin("SERVER <--- ACKNOWLEDGE                ---> " + clientAddress)
									PrintSendToAdmin("SERVER <--- KEY EXCHANGE FINISHED      ---> " + clientAddress)
									keyExchangeFinished = True
								else:
									PrintSendToAdmin("SERVER <-#- [ERRNO 06] ISID             -#-> " + clientAddress)
									# JUMP TO FINALLY AND FINISH CONNECTION
									return
							# INFORMATION / ADMINISTRATIVE PACKETS
							elif packetID == "INF" and keyExchangeFinished:
								if packetSID == "BGN":
									# BEGIN TRANSACTION
									PrintSendToAdmin("SERVER <--- BEGIN TRANSACT.            <--- " + clientAddress)
									# TODO: OPEN CONNECTION TO DATABASE
								elif packetSID == "END":
									# COMMIT TRANSACTION
									PrintSendToAdmin("SERVER <--- COMMIT TRANSACT.           <--- " + clientAddress)
									# TODO: COMMIT CHANGES TO DATABASE
								elif packetSID == "CNG":
									# UPDATE ACCOUNT --> NEEDS (VALID CID), UNAME, PWD --> EMAIL CONFIRMATION
									return
								elif packetSID == "DEL":
									# DELETE CLIENTS DATA --> EMAIL CONFIRMATION
									return
								elif packetSID == "ERR":
									# SOMETHING WENT WRONG
									return
								elif packetSID == "ACK":
									# ACKNOWLEDGEMENT PACKET
									return
								else:
									PrintSendToAdmin("SERVER <-#- [ERRNO 06] ISID             -#-> " + clientAddress)
							# REQUEST PACKETS
							elif packetID == "REQ" and keyExchangeFinished:
								# INSERT REQUEST
								if packetSID == "INS":
									PrintSendToAdmin("SERVER <--- DB-QUERY: INSERT DATA      <--- " + clientAddress)
									dbThread = Thread(target = DatabaseManagement.Insert, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									dbThread.start()
								# UPDATE REQUEST
								elif packetSID == "UPD":
									PrintSendToAdmin("SERVER <--- DB-QUERY: UPDATE DATA      <--- " + clientAddress)
									dbThread = Thread(target = DatabaseManagement.Update, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									dbThread.start()
								# SELECT REQUEST
								elif packetSID == "SEL":
									PrintSendToAdmin("SERVER <--- DB-QUERY: SELECT DATA      <--- " + clientAddress)
									dbThread = Thread(target = DatabaseManagement.Select, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									dbThread.start()
								# HEADER REQUEST FOR DATABASE SYNCRONIZATION
								elif packetSID == "SYN":
									PrintSendToAdmin("SERVER <--- DB-QUERY: SYNCHRONIZE      <--- " + clientAddress)
									dbThread = Thread(target = DatabaseManagement.Sync, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									dbThread.start()
								# DELETE REQUEST
								elif packetSID == "DEL":
									PrintSendToAdmin("SERVER <--- DB-QUERY: DELETE DATA      <--- " + clientAddress)
									dbThread = Thread(target = DatabaseManagement.Delete, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									dbThread.start()
								else:
									PrintSendToAdmin("SERVER <-#- [ERRNO 06] ISID             -#-> " + clientAddress)
									# JUMP TO FINALLY AND FINISH CONNECTION
									return
							# ACCOUNT MANAGEMENT PACKETS 
							elif packetID == "MNG" and keyExchangeFinished:
								# ADMIN LOGIN
								if packetSID == "ADM":
									PrintSendToAdmin("SERVER <--- LOGIN AS ADMIN             <--- " + clientAddress)
									mgmtThread = Thread(target = Management.LoginAdmin, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# USER REGISTRATION
								elif packetSID == "REG":
									PrintSendToAdmin("SERVER <--- REGISTER NEW USER          <--- " + clientAddress)
									# REGISTER IN DATABASE
									dbThread = Thread(target = Management.Register, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									dbThread.start()
								# USER LOGIN
								elif packetSID == "LGI":
									PrintSendToAdmin("SERVER <--- LOGIN REQUEST              <--- " + clientAddress)
									dbThread = Thread(target = Management.Login, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									dbThread.start()
								# REMOTE SERVER SHUTDOWN
								elif packetSID == "SHT":
									PrintSendToAdmin("SERVER <--- SHUTDOWN REQUEST           <--- " + clientAddress)
									mgmtThread = Thread(target = Management.Shutdown, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# REMOTE SERVER REBOOT
								elif packetSID == "RBT":
									PrintSendToAdmin("SERVER <--- REBOOT REQUEST             <--- " + clientAddress)
									mgmtThread = Thread(target = Management.Reboot, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# DUMP LOGS
								elif packetSID == "LOG":
									PrintSendToAdmin("SERVER <--- LOG DUMP REQUEST           <--- " + clientAddress)
									mgmtThread = Thread(target = Management.DumpEventLog, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# USER LOGOUT
								elif packetSID == "LGO":
									PrintSendToAdmin("SERVER <--- LOGOUT REQUEST             <--- " + clientAddress)
									mgmtThread = Thread(target = Management.Logout, args = (clientAddress, clientSocket, aesKey, False))
									mgmtThread.start()
								# LIST CONNECTED CLIENTS
								elif packetSID == "LIC":
									PrintSendToAdmin("SERVER <--- DISPLAY CLIENTS            <--- " + clientAddress)
									mgmtThread = Thread(target = Management.ListClients, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# KICK CLIENT(S)
								elif packetSID == "KIK":
									PrintSendToAdmin("SERVER <--- KICK CLIENT                <--- " + clientAddress)
									mgmtThread = Thread(target = Management.Kick, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# ACCOUNT REQUESTS
								elif packetSID == "ACR":
									# INITIALIZE PASSWORD CHANGE
									if decryptedData[6:9] == "PWC":
										PrintSendToAdmin("SERVER <--- INITIALIZE PASSWORD CHANGE <--- " + clientAddress)
										mgmtThread = Thread(target = Management.AccountRequest, args = (decryptedData[9:], clientAddress, clientSocket, aesKey))
										mgmtThread.start()
									# INITAILIZE ACCOUNT DELETION
									elif decryptedData[6:9] == "DEL":
										PrintSendToAdmin("SERVER <--- INITIALIZE DELETE ACCOUNT  <--- " + clientAddress)
										mgmtThread = Thread(target = Management.AccountRequest, args = (decryptedData[9:], clientAddress, clientSocket, aesKey))
										mgmtThread.start()
									else:
										PrintSendToAdmin("SERVER <-#- [ERRNO 06] ISID             -#-> " + clientAddress)
										# JUMP TO FINALLY AND FINISH CONNECTION
										return
								# COMMIT PASSWORD CHANGE
								elif packetSID == "CPC":
									PrintSendToAdmin("SERVER <--- COMMIT PASSWORD CHANGE     <--- " + clientAddress)
									mgmtThread = Thread(target = Management.PasswordChange, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# GET NEW COOKIE
								elif packetSID == "CKI":
									PrintSendToAdmin("SERVER <--- REQUEST NEW COOKIE         <--- " + clientAddress)
									mgmtThread = Thread(target = Management.RequestCookie, args = (clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# PROVIDE VALIDATION CODE FOR NEW DEVICE
								elif packetSID == "CND":
									PrintSendToAdmin("SERVER <--- VALIDATE NEW DEVICE        <--- " + clientAddress)
									mgmtThread = Thread(target = Management.LoginNewDevice, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# PROVIDE ACTIVATION CODE FOR ACCOUNT
								elif packetSID == "VER":
									PrintSendToAdmin("SERVER <--- ACTIVATE ACCOUNT           <--- " + clientAddress)
									mgmtThread = Thread(target = Management.EmailVerification, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# PROVIDE VALIDATION CODE TO DELETE ACCOUNT
								elif packetSID == "DEL":
									PrintSendToAdmin("SERVER <--- DELETE ACCOUNT             <--- " + clientAddress)
									mgmtThread = Thread(target = Management.DeleteAccount, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# BAN CLIENT
								elif packetSID == "BAN":
									PrintSendToAdmin("SERVER <--- BAN USER BY IP             <--- " + clientAddress)
									mgmtThread = Thread(target = Management.Ban, args = (decryptedData[6:], clientAddress, clientSocket, aesKey, False))
									mgmtThread.start()
								# BAN ACCOUNT
								elif packetSID == "BNA":
									PrintSendToAdmin("SERVER <--- BAN ACCOUNT                <--- " + clientAddress)
									mgmtThread = Thread(target = Management.BanAccount, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# PROVIDE VALIDATION CODE FOR NEW ADMIN DEVICE
								elif packetSID == "NAD":
									PrintSendToAdmin("SERVER <--- VALIDATE NEW ADMIN DEVICE  <--- " + clientAddress)
									mgmtThread = Thread(target = Management.LoginNewAdmin, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# INITIALIZE ADMIN PASSWORD CHANGE
								elif packetSID == "APR":
									PrintSendToAdmin("SERVER <--- INITIALIZE ADMIN-PW CHANGE <--- " + clientAddress)
									mgmtThread = Thread(target = Management.AdminPasswordChangeRequest, args = (clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								# COMMIT ADMIN PASSWORD CHANGE
								elif packetSID == "APC":
									PrintSendToAdmin("SERVER <--- COMMIT ADMIN-PW CHANGE     <--- " + clientAddress)
									mgmtThread = Thread(target = Management.AdminPasswordChange, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
									mgmtThread.start()
								else:
									PrintSendToAdmin("SERVER <-#- [ERRNO 06] ISID             -#-> " + clientAddress)
									# JUMP TO FINALLY AND FINISH CONNECTION
									return
						else:
							# INVALID PACKET SPECIFIER
							PrintSendToAdmin("SERVER <-#- [ERRNO 05] IPSP             -#-> " + clientAddress)
							return
					else:
						return
		except SocketError as SOCKET_ERROR:
			if SOCKET_ERROR.errno == errno.ETIMEDOUT:
				isTimedOut = True
			else:
				# SET SOCKET ERROR FLAG
				isSocketError = True
		# FREE THE SOCKET ONCE THE CLIENT DISCONNECTS OR THE CONNECTION FAILS
		finally:
			# TODO: FIX ANY LOGOUT / DISCONNECT ERRORS (BROKEN PIPE / TRANSPORT)
			# SERVER HAS BEEN STOPPED
			if Server.stopped or clientSocket.fileno() == -1:
				exit()
			# CHECK IF SOCKET ERROR OCCURED
			elif isSocketError:
				# CLOSE THE CONNECTION
				clientSocket.close()
				# REMOVE CLIENT FROM ALL LISTS
				Management.Logout(clientAddress, clientSocket, aesKey, True)
				Management.Unlist(clientSocket)
				PrintSendToAdmin("SERVER <-x- DISCONNECTED: ERROR        -x-> " + clientAddress)
			# CHECK IF A TCP FIN HASH BEEN RECEIVED
			elif isTcpFin:
				# CHECK IF DISCONNECT MESSAGE HAS ALREADY BEEN SHOWN
				disconnectMessageShowed = True
				# ITERATE OVER ALL LISTE CLIENTS
				for client in Server.allClients:
					# IF THERE IS NOT MATCH THERE IS NO NEED TO SHOW THE MESSAGE AGAIN
					if clientSocket in client:
						disconnectMessageShowed = False
						break
				# REMOVE CLIENT FROM LISTING
				Management.Unlist(clientSocket)
				Management.Logout(clientAddress, clientSocket, aesKey, True)
				# SEND TCP FIN
				clientSocket.shutdown(socket.SHUT_RDWR)
				# CLOSE SOCKET
				clientSocket.close()
				# CHECK IF MESSAGE FLAG HAS BEEN SET
				if not disconnectMessageShowed:
					# SHOW DISCONNECTED-MESSAGE
					PrintSendToAdmin ("SERVER <-x- DISCONNECTED               -x-> " + clientAddress)
			elif isTimedOut:
				clientSocket.close()
				# REMOVE CLIENT FROM ALL LISTS
				Management.Logout(clientAddress, clientSocket, aesKey, True)
				Management.Unlist(clientSocket)
				PrintSendToAdmin("SERVER <-x- DISCONNECTED: TIMEOUT      -x-> " + clientAddress)
			else:
				# LOGOUT CLIENT
				if isDisconnected:
					# REMOVE CLIENT FROM LISTING
					Management.Unlist(clientSocket)
					# REMOVE CLIENT FROM AUTHORIZED CLIENTS
					Management.Logout(clientAddress, clientSocket, aesKey, False)
					# SEND TCP FIN
					clientSocket.shutdown(socket.SHUT_RDWR)
					# CLOSE SOCKET
					clientSocket.close()
					PrintSendToAdmin ("SERVER <-x- DISCONNECTED               -x-> " + clientAddress)
				elif not Server.running:
					Management.Logout(clientAddress, clientSocket, aesKey, True)
					Management.Disconnect(clientSocket, "SERVER_SHUTDOWN", clientAddress, False)
				else:
					Management.Logout(clientAddress, clientSocket, aesKey, True)
					Management.Disconnect(clientSocket, "", clientAddress, True)
		
# INITIALIZE THE SERVER
ServerThread = Server()
ServerThread.start()
