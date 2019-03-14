#!/usr/bin/python

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
VERSION = "0.3-2a.19"
BUILD = "unstable"
DATE = "Mar 02 2019"
TIME = "22:24:14"


################################################################################
#------------------------------------IMPORTS-----------------------------------#
################################################################################
try:
	# os IS NEEDED TO CLEAR THE CONSOLE. SO IMPORT IT FIRST
	import os
except Exception as e:
	# IF THE PYTHON INSTALLATION IS MESSED UP EXIT HERE AND PRINT ERROR MESSAGE
	print(e)
	exit()
# CHECK IF RUNNING IN MICROSOFT-WINDOWS ENVIRONMENT 
if os.name == "nt":
	# HELL NO! IT'S WINDOWS :O  NOTHING'S GONNA WORK--> EXIT
	print("FATAL: This program is not designed to be run on windows!")
	exit()
# CLEAR THE CONSOLE
os.system("clear")
print(CWHITE + "         Importing required packages ..." + ENDF)
# IMPORT PACKAGES
try:
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
	import scrypt
	import sqlite3
	import base64
	import argparse
	import secrets
	import hashlib
	import glob
	import datetime
	from datetime import datetime as datetimealt
	import time
	import subprocess
	import select
	import re
	import errno
	import smtplib
	import pygeoip
	import sys
	import getpass
	import inspect
	from config import *
	from email.mime.multipart import MIMEMultipart
	from email.mime.text import MIMEText
	from email.mime.image import MIMEImage
except Exception as e:
	# IF ANYTHING GOES WRONG PRINT ERROR MESSAGE AND EXIT
	print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Importing required packages: " + str(e) + ENDF)
	exit()
else:
	# ALL IMPORTS DONE
	print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Imported required packages." + ENDF)

################################################################################
#------------------------------SERVER CRYPTO CLASS-----------------------------#
################################################################################


# PROVIDES CRYPTOGRAPHIC METHODS
class CryptoHelper():
	
	def GenerateHMACkey(aesKey, nonce, clientSocket):
		hmacKey = hashlib.sha256(bytes(aesKey + nonce.replace("\x00", ""),"utf-8")).hexdigest()
		for client in Server.allClients:
			if clientSocket in client:
				client[4] = hmacKey
				break
				
	def CalculateHMAC(k1, k2, message):
		return CryptoHelper.SHA256(k2 + CryptoHelper.SHA256(k1 + message))
		
	def VerifyHMAC(k1, k2, fullMessage):
		hmac = fullMessage[-44:]
		message = fullMessage[:-44]
		actualHMAC = CryptoHelper.SHA256(k2 + CryptoHelper.SHA256(k1 + message))
		if hmac == actualHMAC:
			return True
		else:
			return False
			
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
		return scrypt.hash(plaintext, salt, 262144, 8, 1, 128).hex()
		
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
		# SET BLOCKSIZE FOR PADDING TO 16 BYTES (128 BITS)
		self.bs = 16
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
		return str(base64.b64encode(iv + cipher.encrypt(raw)))[2:-1]
		
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

	# ADD PADDING
	def _pad(self, s):
		return s + (self.bs - len(s) % self.bs) * chr(self.bs - len(s) % self.bs).encode("ASCII")
		
	# REMOVE PADDING
	@staticmethod
	def _unpad(s):
		return s[:-ord(s[len(s)-1:])]

# PROVIDES NETWORK RELATED METHODS
class Network():
	
	# SEND DATA USING ENCRYPTION
	def SendEncrypted(clientSocket, aesKey, data):
		try:
			aesEncryptor = AESCipher(aesKey)
			encryptedData = aesEncryptor.encrypt(data)
			hmacKeys = GetHMACkeys(clientSocket)
			clientSocket.send(b'\x01' + bytes("E" + encryptedData + CryptoHelper.CalculateHMAC(hmacKeys[0], hmacKeys[1], encryptedData), "utf-8") + b'\x04')
		except SocketError as e:
			Log.ServerEventLog("SOCKET_ERROR", e)
	
	# SEND DATA WITHOUT ENCRYPTION
	def Send(clientSocket, data):
		clientSocket.send(b'\x01' + bytes("U" + data, "utf-8") + b'\x04')
		
			

# PROVIDES DATABASE RELATED METHODS
class DatabaseManagement():
	
	# CLEANS UP THE DATABASE
	def GarbageCollection():
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# INITIALIZE VARIABLES
		users = None
		blacklist = None
		# QUERY DATABASE AND STORE FETCHED RUSULTS IN PREVIOUSLY INITIALIZED VARIABLES
		try:
			cursor.execute("SELECT B_id, B_time, B_duration FROM Tbl_blacklist;")
			blacklist = cursor.fetchall()
			cursor.execute("SELECT U_id, U_isVerified, U_codeType, U_codeTime FROM Tbl_user;")
			users = cursor.fetchall()
		# THROW SQL EXCEPTION AND LOG ERROR
		except Exception as e:
			connection.close()
			info = "errno%eq!17!;code%eq!SQLE!;message%eq!" + e +"!;"
			Log.ServerEventLog("ERROR", info)
			return False
		try:
			# ITERATE OVER ALL BLACKLIST ENTRIES
			for entry in blacklist:
				# CHECK IF ENTRY IS EXPIRED
				if int(entry[1]) + int(entry[2]) < int(Timestamp().split(".")[0]):
					B_id = entry[0]
					# DELETE ENTRY
					cursor.execute("DELETE FROM Tbl_blacklist where B_id = " + str(B_id) + ";")
		# THROW EXCEPTION AND LOG ERROR
		except Exception as e:
			connection.close()
			info = "errno%eq!00!;code%eq!UNKN!;message%eq!" + e +"!;"
			Log.ServerEventLog("ERROR", info)
			return False
		try:
			# ITERATE OVER ALL ACCOUNTS
			for user in users:
				# CHECK IF ACCOUNBT HAS NOT BEEN ACTIVATED AND IF THE CODE IS EXPIRED
				if user[1] == 0 and user[2] == "ACTIVATE_ACCOUNT" and int(user[3]) + ACCOUNT_ACTIVATION_MAX_TIME < int(Timestamp().split(".")[0]):
					U_id = entry[0]
					# DELETE ACCOUNT
					cursor.execute("DELETE FROM Tbl_user WHERE U_id = " + str(U_id) + ";")
		# THROW EXCEPTION AND LOG ERROR
		except Exception as e:
			info = "errno%eq!00!;code%eq!UNKN!;message%eq!" + e +"!;"
			Log.ServerEventLog("ERROR", info)
			return False
		else:
			# EXIT THREAD
			return True
		finally:
			# FREE RESOURCES
			connection.close()

			
	# INSERT DATA 
	def Insert(command, clientAddress, clientSocket, aesKey):
		
		# SPLIT THE RAW COMMAND TO GET THE CREDENTIALS
		# EXAMPLE COMMAND
		# local_id%eq!local_id!;uname%eq!uname!;password%eq!password!;...
		queryParameters = command.split(";")
		# CHECK FOR SECURITY ISSUES
		if not DatabaseManagement.Security(queryParameters, clientAddress, clientSocket, aesKey):
			return
		# SECURITY CHECK PASSED
		# CHECK CREDENTIALS
		userID = Management.CheckCredentials(clientAddress, clientSocket, aesKey)
		if not userID:
			return
		# CREDENTIAL CHECK PASSED
		# FORMAT PARAMETERS
		localID = None
		try:
			for param in queryParameters:
				if "local_id" in param:
					localID = param.split("%eq")[1].replace("!","")
		except Exception as e:
			Handle.Error("ICMD", "INVALID_FORMATTING: " + e, clientAddress, clientSocket, aesKey, True)
			return
		if not localID:
			Handle.Error("ICMD", "TOO_FEW_PARAMETERS", clientAddress, clientSocket, aesKey, True)
			return
		# INITIALIZE VARAIBLES
		qUsername = None
		qPassword = None
		qHost = None
		qEmail = None
		qNotes = None
		query = ""
		values = ""
		# ITERATE OVER QUERY PARAMETERS AND SET VARIABLES
		try:
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
		except Exception as e:
			Handle.Error("ICMD", "INVALID_FORMATTING: " + e, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF ANY PARAMETERS HAVE BEEN SET
		if query == "":
			# THROW INVALID SQL QUERY PARAMETERS EXCEPTION
			Handle.Error("ISQP", None, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK PASSED: REQUEST IS VALID
		# APPEND USER ID TO QUERY
		query += "," + "D_userid"
		values += "," + str(userID)
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		try:
			# EXECUTE SQL QUERY
			fullQuery = "INSERT INTO Tbl_data (" + query + ") VALUES (" + values + ");"
			# PrintSendToAdmin(fullQuery)
			cursor.execute(fullQuery)
		except Exception as e:
			connection.rollback()
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES
			connection.commit()
		HID = None
		try:
			cursor.execute("SELECT last_insert_rowid() FROM Tbl_data;")
			# CREATE HID (HASHED ID)
			dataID = cursor.fetchall()
			if len(dataID) == 0:
				return
			HID = dataID[0][0]
		except Exception as e:
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		if HID == None:
			Handle.Error("SQLE", None, clientAddress, clientSocket, aesKey, True)
			return
		hashedID = CryptoHelper.BLAKE2(str(HID), str(userID))
		try:
			# ADD HID TO ENTRY
			cursor.execute("UPDATE Tbl_data SET d_hid = \"" + hashedID + "\" WHERE D_id = " + str(HID) + ";")
		except Exception as e:
			connection.rollback()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES
			connection.commit()
		finally:
			# FREE RESOURCES
			connection.close()
		Log.ClientEventLog("INSERT", clientSocket)
		# SEND ACKNOWLEDGEMENT TO CLIENT
		returnData = "DTARETINSlocal_id%eq!" + localID + "!;hashed_id%eq!" + hashedID + "!;"
		Network.SendEncrypted(clientSocket, aesKey, returnData)
		PrintSendToAdmin("SERVER ---> RETURNED STATUS            ---> " + clientAddress)
			
		
	# UPDATE DATA	
	def Update(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE command = "url%eq!url!;host%eq!host!;uname%eq!username!;password%eq!password!;email%eq!email!;notes%eq!notes!;datetime%eq!datetime!;hid%eq!hid!;"
		# SPLIT THE RAW COMMAND TO GET THE PARAMETERS
		queryParameters = command.split(";")
		# CHECK FOR SECURITY ISSUES
		if not DatabaseManagement.Security(queryParameters, clientAddress, clientSocket, aesKey):
			return
		# SECURITY CHECK PASSED
		# CHECK CREDENTIALS
		userID = Management.CheckCredentials(clientAddress, clientSocket, aesKey)
		if userID == None:
			Handle.Error("NLGI", None, clientAddress, clientSocket, aesKey, True)
			return
		# CREDENTIAL CHECK PASSED
		# FORMAT PARAMETERS
		query = ""
		HID = None
		try:
			# ITERATE OVER QUERY PARAMETERS AND SET VARIABLES
			for rawParameter in queryParameters:
				if not len(rawParameter) == 0:
					parameters = rawParameter.split("%eq")
					column = parameters[0]
					value = parameters[1].replace('!','')
					if column in ["uname","password","host","notes","email","datetime","url"]:
						if query == "":
							query = "D_" + column + " = \"" + value + "\""
						else:
							query += ",D_" + column + " = \"" + value + "\""
					elif column == "hid":
						HID = value
		except Exception as e:
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
			return
		if not HID:
			Handle.Error("UNKN", None, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF ANY PARAMETERS HAVE BEEN SET
		if query == "":
			# THROW INVALID SQL QUERY PARAMETERS EXCEPTION
			Handle.Error("ISQP", None, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK PASSED: REQUEST IS VALID
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		try:
			# EXECUTE QUERY
			cursor.execute("UPDATE Tbl_data SET " + query + " WHERE D_userid = " + userID + " AND d_hid = \"" + HID + "\";")
		except Exception as e:
			connection.rollback()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES
			connection.commit()
		finally:
			# FREE RESOURCES
			connection.close()
		Log.ClientEventLog("UPDATE", clientSocket)
		# SEND ACKNOWLEDGEMENT TO CLIENT
		returnData = "DTARETUPDhashed_id%eq!" + HID + "!;"
		Network.SendEncrypted(clientSocket, aesKey, returnData)
		PrintSendToAdmin("SERVER ---> RETURNED STATUS            ---> " + clientAddress)
		
	# SELECT DATA FROM THE DATABASE
	def Select(queryParameter, clientAddress, clientSocket, aesKey):
		# EXAMPLE command = "HID1;HID2;HID3;HID4;HID5;"
		# CHECK FOR SECURITY ISSUES
		queryParameters = queryParameter.split(";")
		if not DatabaseManagement.Security(queryParameters, clientAddress, clientSocket, aesKey):
			return
		# SECURITY CHECK PASSED
		# CHECK CREDENTIALS
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
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		rawData = None
		try:
			cursor.execute("SELECT D_host, D_uname, D_password, D_email, D_notes, D_hid, D_datetime FROM Tbl_data WHERE D_userid = " + userID + " AND (" + query + ");")
			# GET DATA FROM CURSOR OBJECT
			rawData = cursor.fetchall()
		except Exception as e:
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		finally:
			# FREE RESOURCES
			connection.close()
		try:
			# CREATE AND SEND PACKET FOR EACH RETURNED ROW
			for entry in rawData:
				# GET VALUES FROM DATA ARRAY
				dHost = str(entry[0])
				dUname = str(entry[1])
				dPassword = str(entry[2])
				dEmail = str(entry[3])
				dNotes = str(entry[4])
				dHid = str(entry[5])
				dDatetime = str(entry[6])
				# APPLY PACKET FORMATTING
				# EXAMPLE: host%eq!test!;uname%eq!test!;password%eq!test!;email%eq!test!;notes%eq!test!;hid%eq!test!;datetime%eq!test!;
				data = "host%eq!" + dHost + "!;uname%eq!" + dUname + "!;password%eq!" + dPassword + "!;email%eq!" + dEmail + "!;notes%eq!" + dNotes + "!;hid%eq!" + dHid + "!;datetime%eq!" + dDatetime + "!;"
				# RETURN DATA TO CLIENT
				returnData = "DTARETSEL" + data
				Network.SendEncrypted(clientSocket, aesKey, returnData)
				PrintSendToAdmin("SERVER ---> RETURNED DATA              ---> " + clientAddress)
			Log.ClientEventLog("SELECT", clientSocket)
			# SEND ACKNOWLEDGEMENT TO CLIENT (LAST PACKET OUT)
			returnData = "INFRETSELACK"
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
			PrintSendToAdmin("SERVER ---> RETURNED STATUS            ---> " + clientAddress)
		except Exception as e:
			Handle.Error("UNKN", e, clientAddress, clientSocket, aesKey, True)
			return
	
	def Delete(queryParameter, clientAddress, clientSocket, aesKey):
		# EXAMPLE command = "HID1;HID2;HID3;HID4;HID5;"
		queryParameters = queryParameter.split(";")
		# CHECK FOR SECURITY ISSUES
		if not DatabaseManagement.Security(queryParameters, clientAddress, clientSocket, aesKey):
			return
		# SECURITY CHECK PASSED
		# CHECK CREDENTIALS
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
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		try:	
			cursor.execute("DELETE FROM Tbl_data WHERE D_userid = " + userID + " AND (" + query + ");")
		except Exception as e:
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			connection.commit()
			Log.ClientEventLog("DELETE", clientSocket)
			# SEND ACKNOWLEDGEMENT TO CLIENT
			returnData = "DTARETDELCONFIRMED" + queryParameter
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
			PrintSendToAdmin("SERVER ---> RETURNED STATUS            ---> " + clientAddress)
		finally:
			# FREE RESOURCES
			connection.close()
			
	def Sync(queryParameter, clientAddress, clientSocket, aesKey):
		# EXAMPLE command = "fetch_mode%eq!FETCH_SYNC!"
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
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# SYNC-MODE ONLY FETCHES IDS 
		if fetchMode == "FETCH_SYNC":
			data = None
			try:
				cursor.execute("SELECT D_hid,D_datetime FROM Tbl_data WHERE D_userid = " + userID + ";")
				data = cursor.fetchall()
			except Exception as e:
				# FREE RESOURCES
				connection.close()
				Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
				return
			if not data:
				# FREE RESOURCES
				connection.close()
				Handle.Error("UNKN", None, clientAddress, clientSocket, aesKey, True)
				return
			# FREE RESOURCES
			connection.close()
			finalData = str(data).replace(", ",",").replace('[','').replace(']','')
			# SEND ACKNOWLEDGEMENT TO CLIENT (LAST PACKET OUT)
			returnData = "DTARETSYN" + finalData
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
			PrintSendToAdmin("SERVER ---> RETURNED SYNDATA           ---> " + clientAddress)
			Log.ClientEventLog("SYNC", clientSocket)
		# DUMP-MODE RETURNS ALL DATA
		elif fetchMode == "FETCH_ALL":
			data = None
			try:
				cursor.execute("SELECT * FROM Tbl_data WHERE D_userid = " + userID + ";")
				data = cursor.fetchall()
			except Exception as e:
				# FREE RESOURCES
				connection.close()
				Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
				return
			if not data:
				# FREE RESOURCES
				connection.close()
				Handle.Error("UNKN", None, clientAddress, clientSocket, aesKey, True)
				return
			# FREE RESOURCES
			connection.close()
			finalData = str(data).replace(", ",",").replace('[','').replace(']','')
			# SEND ACKNOWLEDGEMENT TO CLIENT (LAST PACKET OUT)
			returnData = "DTARETSYN" + finalData
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
			PrintSendToAdmin("SERVER ---> RETURNED DTADUMP           ---> " + clientAddress)
			Log.ClientEventLog("DATA_DUMP", clientSocket)
		else:
			# FREE RESOURCES
			connection.close()
			# THROW INVALID SQL QUERY PARAMETERS EXCEPTION
			Handle.Error("ISQP", "INVALID_MODE", clientAddress, clientSocket, aesKey, True)
			return
		
	# CHECKS FOR SECURITY ISSUES
	def Security(queryArray, clientAddress, clientSocket, aesKey):
		# CHECK FOR SQL INJECTION
		for element in queryArray:
			if ('\"' in element) or ('\'' in element):
				Handle.Error("SQLI", None, clientAddress, clientSocket, aesKey, True)
				Log.ClientEventLog("SQL_INJECTION_ATTEMPT", clientSocket)
				Log.ServerEventLog("SQL_INJECTION_ATTEMPT", GetDetails(clientSocket))
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
	def SetDetails(address, clientSocket):
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
			if clientSocket in client:
				Server.allClients[index][3] = details
	
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
		# [ERRNO 28] ACCA				--> ACCOUNT ALREADY ACTIVATED
		# [ERRNO 29] ADEL				--> ACCOUNT DELETED
		# [ERRNO 30] E2RE				--> EXPIRED 2FA REQUEST
		# [ERRNO 31] IMAC				--> INVALID MESSAGE AUTHENTICATION CODE
		# [ERRNO 32] MAIL				--> ERROR OCCURED DURING SendMail()
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
		elif errorID == "ACCA":
			errorNo = "28"
			if not message:
				message = "ACCOUNT_ALREADY_ACTIVATED"
		elif errorID == "ADEL":
			errorNo = "29"
			if not message:
				message = "ACCOUNT_DELETED_TOO_MANY_REQUESTS"
		elif errorID == "E2RE":
			errorNo = "30"
			if not message:
				message = "EXPIRED_2FA_REQUEST"
		elif errorID == "IMAC":
			errorNo = "31"
			if not message:
				message = "INVALID_MESSAGE_AUTHENTICATION_CODE"
		elif errorID == "MAIL":
			errorNo = "32"
			if not message:
				message = "SEND_MAIL_FAILED"
		else:
			return
		info = "errno%eq!" + errorNo + "!;code%eq!" + errorID + "!;message%eq!" + str(message) +"!;"
		Log.ServerEventLog("ERROR", info)
		PrintSendToAdmin("SERVER <-#- [ERRNO " + errorNo + "] " + errorID + "            -#-> " + clientAddress)
		if not clientSocket:
			PrintSendToAdmin("ERROR MESSAGE: " + info)
			return
		if not isEncrypted:
			Network.Send(clientSocket, "INFERR" + info)
			return
		returnData = "INFERR" + info
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
		
# CONTAINS EVERYTHING ACCOUNT AND SERVER RELATED
class Management():
	
	# CHECK IF COOKIE EXISTS
	def CheckCookie(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE command = "username%eq!username!;password%eq!password!;cookie%eq!cookie!;"
		# SPLIT THE RAW COMMAND TO GET THE CREDENTIALS
		creds = command.split(";")
		# CHECK FOR SECURITY ISSUES
		if not DatabaseManagement.Security(creds, clientAddress, clientSocket, aesKey):
			return
		# INITIALIZE VARIABLES TO STORE CREDENTIALS IN
		cookie = None
		# GET THE COOKIE FROM ARRAY
		try:
			for credential in creds:
				if credential:
					if "cookie" in credential:
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
		if not cookie:
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
			return
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		cookieExists = 0
		try:
			# CHECK IF COOKIE EXISTS AND CONNECTION BETWEEN ACCOUNT AND COOKIE IS EXISTENT
			cursor.execute("SELECT EXISTS(SELECT 1 FROM Tbl_cookies WHERE C_cookie = \"" + cookie + "\" LIMIT 1);")
			data = cursor.fetchall()
			cookieExists = data[0][0]
		except Exception as e:
			# SQL ERROR 
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		finally:
			connection.close()
		returnValue = None
		if cookieExists == 1:
			returnValue = "INFRETmsg%eq!COOKIE_DOES_EXIST!;"
			PrintSendToAdmin("SERVER ---> COOKIE CHECK SUCCESSFUL    ---> " + clientAddress)
		else:
			returnValue = "INFRETmsg%eq!COOKIE_DOES_NOT_EXIST!;"
			PrintSendToAdmin("SERVER ---> COOKIE CHECK FAILED        ---> " + clientAddress)
		if not returnValue:
			Handle.Error("00", None, clientAddress, clientSocket, aesKey, True)
			return
		Network.SendEncrypted(clientSocket, aesKey, returnValue)
		
	
	# GET USERNAME AND PASSWORD AND CHECK IF CREDENTIALS ARE VALID --> RETURN BOOL TO CLIENT
	def CredentialCheckProvider(command, clientAddress, clientSocket, aesKey):
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
		if isNewDevice == 1:
			# ABORT IF DEVICE NOT AFFILIATED TO ACCOUNT --> PREVENT BRUTEFORCE
			connection.close()
			Handle.Error("CRED", "DEVICE_NOT_CONNECTED_TO_ACCOUNT", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK CREDENTIALS
		# HASH PASSWORD
		hashedUsername = CryptoHelper.SHA256(username)
		salt = CryptoHelper.SHA256(hashedUsername + password)
		hashedPassword = CryptoHelper.Scrypt(password, salt)
		credentialsValid = 0
		try:
			# CHECK IF CREDENTIALS ARE VALID
			# QUERY FOR USER ID
			cursor.execute("SELECT 1 FROM Tbl_user WHERE U_username = \"" + username + "\" AND U_password = \"" + hashedPassword + "\";")
			# FETCH DATA
			data = cursor.fetchall()
			# GET USER ID
			credentialsValid = data[0][0]
		except:
			Log.ClientEventLog("CREDENTIAL_CHECK_FAILED", clientSocket)
			# RETURN ERROR MESSAGE TO CLIENT
			Handle.Error("CRED", None, clientAddress, clientSocket, aesKey, True)
			return
		finally:
			connection.close()
		if credentialsValid == 0:
			# CREDENTIAL CHECK FAILED
			Log.ClientEventLog("CREDENTIAL_CHECK_FAILED", clientSocket)
			returnData = "INFRETCREDENTIAL_CHECK_FAILED"
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
			PrintSendToAdmin("SERVER ---> CREDENTIAL CHECK FAILED    ---> " + clientAddress)
			return
		else:
			# CREDENTIAL CHECK PASSED
			Log.ClientEventLog("CREDENTIAL_CHECK_SUCCESSFUL", clientSocket)
			returnData = "INFRETmsg%eq!CREDENTIAL_CHECK_SUCCESSFUL!;"
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
			PrintSendToAdmin("SERVER ---> CREDENTIAL CHECK SUCCESS   ---> " + clientAddress)
			
	
	# CHANGES THE USERS NAME THAT IS USED TO PERSONALIZE MESSAGES
	def ChangeName(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE COMMAND
		# new_name%eq!new_name!;
		parameters = command.split(";")
		# CHECK FOR SQL INJECTION
		if not DatabaseManagement.Security(parameters, clientAddress, clientSocket, aesKey):
			return
		# SECURITY CHECK PASSED
		# INITIALIZE VARIABLES TO STORE EXTRACTED DATA IN
		newName = None
		# EXTRACT REQUIRED DATA FROM PARAMETER-ARRAY
		try:
			for parameter in parameters:
				if parameter:
					if "new_name" in parameter:
						newName = parameter.split("!")[1]
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
		# VALIDATE THAT ALL VARIABLES HAVE BEEN SET
		if not newName:
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
			return
		# GET USER ID AND CHACK IF USER IS LOGGED IN
		userID = Management.CheckCredentials(clientAddress, clientSocket, aesKey)
		if not userID:
			# USER IS NOT LOGGED IN
			Handle.Error("NLGI", None, clientAddress, clientSocket, aesKey, True)
			return
		# USER IS LOGGED IN
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		try:
			# UPDATE THE NAME IN THE DATABASE
			cursor.execute("UPDATE Tbl_user SET U_name = \"" + newName + "\" WHERE U_id = " + userID + ";")
		except Exception as e:
			# SOMETHING WENT WRONG --> THROW SQL EXCEPTION AND ROLLBACK
			connection.rollback()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES
			connection.commit()
		finally:
			# FREE RESOURCES
			connection.close()
		# RETURN SUCCESS STATUS TO CLIENT
		returnData = "INFRETstatus%eq!NAME_CHANGED!;"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
		# GET SOME DEBUG OUTPUT HAPPENING
		PrintSendToAdmin("SERVER ---> NAME CHANGED               ---> " + clientAddress)
	
	# ALLOWS TO RESEND ANY 2FA CODE IN CASE SOME ERROR OCCURED
	def ResendCode(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE COMMAND
		# username%eq!username!;name%eq!name!;email%eq!email!;
		parameters = command.split(";")
		# CHECK FOR SQL INJECTION
		if not DatabaseManagement.Security(parameters, clientAddress, clientSocket, aesKey):
			return
		# SECURITY CHECK PASSED
		# INITIALIZE VARIABLES TO STORE EXTRACTED DATA IN
		username = None
		nickname = None
		email = None
		# EXTRACT REQUIRED DATA FROM PARAMETER-ARRAY
		try:
			for parameter in parameters:
				if parameter:
					if "username" in parameter:
						username = parameter.split("!")[1]
					elif "name" in parameter:
						nickname = parameter.split("!")[1]
					elif "email" in parameter:
						email = parameter.split("!")[1]
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
		# VALIDATE THAT ALL VARIABLES HAVE BEEN SET
		if not username or not email or not nickname:
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
			return
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR
		cursor = connection.cursor()
		# INITIALIZE VARIABLES
		codeTime = None
		codeAttempts = None
		codeType = None
		codeResend = None
		isVerified = None
		userID = None
		try:
			# POPULATE VARIABLES WITH VALUES FROM DATABASE
			cursor.execute("SELECT U_codeType, U_codeTime, U_codeAttempts, U_codeResend, U_isVerified, U_id FROM Tbl_user WHERE U_username = \"" + username + "\" AND U_name = \"" + nickname + "\" AND U_email = \"" + email + "\";")
			data = cursor.fetchall()
			codeType = data[0][0]
			codeTime = data[0][1]
			codeAttempts = data[0][2]
			codeResend = data[0][3]
			isVerified = data[0][4]
			userID = str(data[0][5])
		except Exception as e:
			# IN CASE OF ERROR FREE RESOURCES AND THROW SQL EXCEPTION
			connection.close()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# VERIFY THAT ALL VARIABLES HAVE BEEN SET
		if not codeType or not codeTime or codeAttempts == None or codeResend == None or isVerified == None or not userID:
			# SOMETHING WENT WRONG --> TROW SQL EXCEPTION AND FREE RESOURCES
			connection.close()
			Handle.Error("SQLE", None, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF CODE EVENT IS AVTIVE
		if codeAttempts == -1:
			# FREE RESOURCES
			connection.close()
			Handle.Error("NCES", None, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF ACCOUNT IS ACVIVATED
		if not codeResend == -1 and isVerified == 0:
			# CHECK IF THE MAXIMUM AMOUNT OF RESEND CALLS HAS BEEN REACHED
			if codeResend + 1 > RESEND_CODE_MAX_COUNT:
				# DELETE ACCOUNT --> (USER HAS NEVER BEEN LOGGED IN BEFORE)
				Handle.Error("ADEL", None, clientAddress, clientSocket, aesKey, True)
				try:
					# DELETE ALL DATA ASSOCIATED TO THE USERS ACCOUNT
					cursor.execute("DELETE FROM Tbl_connectUserCookies WHERE U_id = " + userID + ";")
					cursor.execute("DELETE FROM Tbl_data WHERE D_userid = " + userID + ";")
					cursor.execute("DELETE FROM Tbl_clientLog WHERE L_userid = " + userID + ";")
					cursor.execute("DELETE FROM Tbl_user WHERE U_id = " + userID + ";")
				except:
					# SOMETHING WENT WRONG --> LOG ERROR MESSAGE AND ROLLBACK
					connection.rollback()
					Handle.Error("SQLE", None, clientAddress, clientSocket, aesKey, True)
					return
				else:
					# ALL GOOD --> COMMIT CHANGES
					connection.commit()
				finally:
					# FREE RESOURCES
					connection.close()
					return
			else:
				# INCREMENT RESEND-CALL-COUNTER BY 1
				try:
					cursor.execute("UPDATE Tbl_user SET U_codeResend = " + str(codeResend + 1) + " WHERE U_id = " + userID + ";")
				except Exception as e:
					# SOMETHING WENT WRONG --> LOG ERROR MESSAGE AND ROLLBACK
					connection.rollback()
					# FREE RESOURCES
					connection.close()
					Handle.Error("SQLE", None, clientAddress, clientSocket, aesKey, True)
					return
				else:
					# COMMIT CHANGES
					connection.commit()
		# ALL UNSPECIFIC CHECKS PASSED
		# GENERATE NEW 2FA CODE
		codeFinal = CodeGenerator()
		# UPDATE DATABASE AND SET NEW CODE
		try:
			cursor.execute("UPDATE Tbl_user SET U_code = \"" + codeFinal + "\" WHERE U_id = " + userID + ";")
		except Exception as e:
			# ROLLBACK IN CASE OF ERROR
			connection.rollback()
			Handle.Error("SQLE", None, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES
			connection.commit()
		finally:
			# FREE RESOURCES --> DATABASE ACCESS NOT NEEDED ANYMORE IN THIS THREAD
			connection.close()
		# INITIALZE VARIABLES FOR SWITCH CASE (MORE LIKE IF ELIF ... BECAUSE PYTHON :P)
		subject = None
		text = None
		html = None
		# GET DETAILS OF CLIENTS MACHINE
		details = GetDetails(clientSocket)
		# ADAPT FORMATTING TO WORK WITH HTML
		htmlDetails = details.replace("\n","<br>")
		# SWITCH CODE TYPE
		if codeType == "ADMIN_PASSWORD_CHANGE":
			# CHECK IF INITIAL CODE REQUEST HAS ALREADY EXPIRED
			if int(codeTime) + PASSWORD_CHANGE_CONFIRMATION_ADMIN_MAX_TIME < int(time.time()) or codeAttempts >= MAX_CODE_ATTEMPTS_ADMIN:
				Handle.Error("E2RE", None, clientAddress, clientSocket, aesKey, True)
				return
			else:
				# SET CODE EVENT SPECIFIC VALUES
				subject = SUPPORT_EMAIL_PASSWORD_CHANGE_ADMIN_SUBJECT
				text = SUPPORT_EMAIL_PASSWORD_CHANGE_ADMIN_PLAIN_TEXT % (details, codeFinal, ConvertFromSeconds((int(codeTime) + PASSWORD_CHANGE_CONFIRMATION_ADMIN_MAX_TIME) - int(time.time())))
				html = SUPPORT_EMAIL_PASSWORD_CHANGE_ADMIN_HTML_TEXT % (htmlDetails, codeFinal, ConvertFromSeconds((int(codeTime) + PASSWORD_CHANGE_CONFIRMATION_ADMIN_MAX_TIME) - int(time.time())))
		elif codeType == "PASSWORD_CHANGE":
			# CHECK IF INITIAL CODE REQUEST HAS ALREADY EXPIRED
			if int(codeTime) + PASSWORD_CHANGE_CONFIRMATION_MAX_TIME < int(time.time()) or codeAttempts >= MAX_CODE_ATTEMPTS:
				Handle.Error("E2RE", None, clientAddress, clientSocket, aesKey, True)
				return
			else:
				# SET CODE EVENT SPECIFIC VALUES
				subject = SUPPORT_EMAIL_PASSWORD_CHANGE_SUBJECT
				text = SUPPORT_EMAIL_PASSWORD_CHANGE_PLAIN_TEXT % (nickname, details, codeFinal, ConvertFromSeconds((int(codeTime) + PASSWORD_CHANGE_CONFIRMATION_MAX_TIME) - int(time.time())))
				html = SUPPORT_EMAIL_PASSWORD_CHANGE_HTML_TEXT % (nickname, htmlDetails, codeFinal, ConvertFromSeconds((int(codeTime) + PASSWORD_CHANGE_CONFIRMATION_MAX_TIME) - int(time.time())))
		elif codeType == "DELETE_ACCOUNT":
			# CHECK IF INITIAL CODE REQUEST HAS ALREADY EXPIRED
			if int(codeTime) + DELETE_ACCOUNT_CONFIRMATION_MAX_TIME < int(time.time()) or codeAttempts >= MAX_CODE_ATTEMPTS:
				Handle.Error("E2RE", None, clientAddress, clientSocket, aesKey, True)
				return
			else:
				# SET CODE EVENT SPECIFIC VALUES
				subject = SUPPORT_EMAIL_DELETE_ACCOUNT_SUBJECT
				text = SUPPORT_EMAIL_DELETE_ACCOUNT_PLAIN_TEXT % (nickname, details, codeFinal, ConvertFromSeconds((int(codeTime) + DELETE_ACCOUNT_CONFIRMATION_MAX_TIME) - int(time.time())))
				html = SUPPORT_EMAIL_DELETE_ACCOUNT_HTML_TEXT % (nickname, htmlDetails, codeFinal, ConvertFromSeconds((int(codeTime) + DELETE_ACCOUNT_CONFIRMATION_MAX_TIME) - int(time.time())))
		elif codeType == "ACTIVATE_ACCOUNT":
			# CHECK IF INITIAL CODE REQUEST HAS ALREADY EXPIRED
			if int(codeTime) + ACCOUNT_ACTIVATION_MAX_TIME < int(time.time()) or codeAttempts >= MAX_CODE_ATTEMPTS:
				Handle.Error("E2RE", None, clientAddress, clientSocket, aesKey, True)
				return
			else:
				# SET CODE EVENT SPECIFIC VALUES
				subject = SUPPORT_EMAIL_REGISTER_SUBJECT
				text = SUPPORT_EMAIL_REGISTER_PLAIN_TEXT % (nickname, codeFinal, ConvertFromSeconds((int(codeTime) + ACCOUNT_ACTIVATION_MAX_TIME) - int(time.time())))
				html = SUPPORT_EMAIL_REGISTER_HTML_TEXT % (nickname, codeFinal, ConvertFromSeconds((int(codeTime) + ACCOUNT_ACTIVATION_MAX_TIME) - int(time.time())))
		elif codeType == "ADMIN_NEW_LOGIN":
			# CHECK IF INITIAL CODE REQUEST HAS ALREADY EXPIRED
			if int(codeTime) + NEW_DEVICE_CONFIRMATION_ADMIN_MAX_TIME < int(time.time()) or codeAttempts >= MAX_CODE_ATTEMPTS_ADMIN:
				Handle.Error("E2RE", None, clientAddress, clientSocket, aesKey, True)
				return
			else:
				# SET CODE EVENT SPECIFIC VALUES
				subject = SUPPORT_EMAIL_NEW_ADMIN_DEVICE_SUBJECT
				text = SUPPORT_EMAIL_NEW_ADMIN_DEVICE_PLAIN_TEXT % (details, codeFinal, ConvertFromSeconds((int(codeTime) + NEW_DEVICE_CONFIRMATION_ADMIN_MAX_TIME) - int(time.time())))
				html = SUPPORT_EMAIL_NEW_ADMIN_DEVICE_HTML_TEXT % (htmlDetails, codeFinal, ConvertFromSeconds((int(codeTime) + NEW_DEVICE_CONFIRMATION_ADMIN_MAX_TIME) - int(time.time())))
		elif codeType == "NEW_LOGIN":
			# CHECK IF INITIAL CODE REQUEST HAS ALREADY EXPIRED
			if int(codeTime) + NEW_DEVICE_CONFIRMATION_MAX_TIME < int(time.time()) or codeAttempts >= MAX_CODE_ATTEMPTS:
				Handle.Error("E2RE", None, clientAddress, clientSocket, aesKey, True)
				return
			else:
				# SET CODE EVENT SPECIFIC VALUES
				subject = SUPPORT_EMAIL_NEW_DEVICE_SUBJECT
				text = SUPPORT_EMAIL_NEW_DEVICE_PLAIN_TEXT % (nickname, details, codeFinal, ConvertFromSeconds((int(codeTime) + NEW_DEVICE_CONFIRMATION_MAX_TIME) - int(time.time())))
				html = SUPPORT_EMAIL_NEW_DEVICE_HTML_TEXT % (nickname, htmlDetails, codeFinal, ConvertFromSeconds((int(codeTime) + NEW_DEVICE_CONFIRMATION_MAX_TIME) - int(time.time())))
		else:
			# UNKNOWN CODE EVENT --> JUST DO NOTHING AND CREATE LOG ENTRY
			Handle.Error("NCES", None, clientAddress, clientSocket, aesKey, True)
			return
		# RETURN SUCCESS STATUS TO CLIENT
		returnData = "INFRETtodo%eq!CODE_RESEND!;"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
		# SEND EMAIL
		Management.SendMail(SUPPORT_EMAIL_SENDER, email, subject, text, html, clientAddress)
		
			
	# RETURN THE CLIENT LOG OF THE LOGGED IN USER
	def GetAccountActivity(clientAddress, clientSocket, aesKey):
		# GET USER ID
		userID = Management.CheckCredentials(clientAddress, clientSocket, aesKey)
		if not userID:
			# USER IS NOT LOGGED IN
			Handle.Error("NLGI", None, clientAddress, clientSocket, aesKey, True)
			return
		# USER IS LOGGED IN
		# CREATE CONNECTION TO DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR OBJECT TO INTERACT WITH DATABASE
		cursor = connection.cursor()
		# INITIALIZE VARIABLE TO STORE RETURNED DATA
		clientLog = None
		try:
			# QUERY THE DATABASE FOR THE ACCOUNT ACTIVITY
			cursor.execute("SELECT L_event, L_ip, L_datetime, L_details FROM Tbl_clientLog, Tbl_user WHERE L_userid = U_id AND U_id = " + userID + ";")
			clientLog = cursor.fetchall()
		except Exception as e:
			# AN SQL ERROR OCCURED --> LOG ERROR
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# CHECK IF DATA HAS BEEN FETCHED PROPERLY
			if clientLog == None:
				# SOMETHONG WENT WRONG --> SQL ERROR
				Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
				return
			# RETURN DATA TO CLIENT
			returnData = "LOGDMP" + str(clientLog)
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
			PrintSendToAdmin("SERVER ---> ACCOUNT ACTIVITY           ---> " + clientAddress)
		finally:
			# FREE RESOURCES
			connection.close()
	
	# CHANGES THE EMAIL ADDRESS IF THE ACCOUNT HAS NOT BEEN ACTIVATED / VERIFIED YET
	def ChangeEmailAddress(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE COMMAND
		# username%eq!testuser1!;cookie%eq!cookie!;new_email%eq!frederik.hoeft@gmail.com!;
		# username%eq!username!;cookie%eq!cookie!;new_email%eq!new_email!;
		parameters = command.split(";")
		# CHECK FOR SQL INJECTION
		if not DatabaseManagement.Security(parameters, clientAddress, clientSocket, aesKey):
			return
		# SECURITY CHECK PASSED
		# INITIALIZE VARIABLES
		username = None
		cookie = None
		newEmail = None
		# EXTRACT VALUES FROM PROVIDED COMMAND
		try:
			for parameter in parameters:
				if "username" in parameter:
					username = parameter.split("!")[1]
				elif "cookie" in parameter:
					cookie = parameter.split("!")[1]
				elif "new_email" in parameter:
					newEmail = parameter.split("!")[1]
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
		# VALIDATE THAT ALL VARIABLES HAVE BEEN INITIALIZED
		if not username or not newEmail or not cookie:
			Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, aesKey, True)
			return
		# CREATE CONNECTION TO SQLITE DATABASE
		connection = sqlite3.connect(Server.dataBase)
		# CREATE CURSOR OBJECT
		cursor = connection.cursor()
		# INITIALIZE VARIABLE TO STORE ACCOUNT STATUS IN
		isVerified = None
		try:
			# QUERY DATABASE FOR ACCOUNT STATUS
			cursor.execute("SELECT U.U_isVerified FROM Tbl_user as U, Tbl_connectUserCookies as CUO, Tbl_cookies as C WHERE U.U_username = \"" + username + "\" and C.C_cookie = \"" + cookie + "\" and U.U_id = CUO.U_id and CUO.C_id = C.C_id;")
			isVerified = cursor.fetchall()[0][0]
		except IndexError:
			# INDEX OUT OF RANGE --> COOKIE IS NOT BOUND TO ACCOUNT / ACCOUNT DOES NOT EXIST / COOKIE DOES NOT EXIST
			# FREE RESOURCES
			connection.close()
			# LOG ERROR
			Handle.Error("PERM", None, clientAddress, clientSocket, aesKey, True)
			return
		except Exception as e:
			# FREE RESOURCES
			connection.close()
			# LOG ERROR
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF VARIABLE HAS BEEN SET AT ALL
		if isVerified == None:
			# FREE RESOURCES
			connection.close()
			# LOG ERROR
			Handle.Error("SQLE", None, clientAddress, clientSocket, aesKey, True)
			return
		# CHACK IF ACCOUNT HAS ALREADY BEEN VERIFIED
		if not isVerified == 0:
			# FREE RESOURCES
			connection.close()
			# LOG ERROR
			Handle.Error("ACCA", None, clientAddress, clientSocket, aesKey, True)
			return
		try:
			# UPDATE EMAIL ADDRESS IN DATABASE
			cursor.execute("UPDATE Tbl_user SET U_email = \"" + newEmail + "\" WHERE U_username = \"" + username + "\";")
		except Exception as e:
			# SOMETHING WENT WRONG --> ROLLBACK
			connection.rollback()
			# LOG ERROR
			Handle.Error("SQLE", None, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES
			connection.commit()
		finally:
			# FREE RESOURCES
			connection.close()
		PrintSendToAdmin("SERVER ---> EMAIL ADDRESS UPDATED      ---> " + clientAddress)
		# RETURN STATUS TO CLIENT
		returnData = "INFRETEMAIL_UPDATED"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
			
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
		if not codeType == "ADMIN_NEW_LOGIN" or codeAttempts == -1:
			Handle.Error("NCES", "NO_NEW_LOGIN_SCHEDULED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF VALIDATION CODE MATCHES PROVIDED CODE
		if not code == providedCode:
			# CHECK IF NUMBER OF WRONG CODES HAS BEEN EXCEEDED
			if codeAttempts + 1 >= MAX_CODE_ATTEMPTS_ADMIN:
				# USER TRIES TO BRUTEFORCE VALIDATION CODE
				Handle.Error("F2FA", None, clientAddress, clientSocket, aesKey, True)
				# BAN DEVICE
				Management.Ban("ip%eq!" + clientAddress.split(":")[0] + "!;duration%eq!" + str(WRONG_CODE_AUTOBAN_DURATION_ADMIN) + "!;", clientAddress, clientSocket, aesKey, True)
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
		if int(Timestamp()) - int(codeTime) > NEW_DEVICE_CONFIRMATION_ADMIN_MAX_TIME:
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
			cursor.execute("SELECT U_id FROM Tbl_user WHERE U_username = \"__ADMIN__\" AND U_password = \"" + hashedPassword + "\" LIMIT 1;")
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
		returnData = "INFRETmsg%eq!SUCCESSFUL_ADMIN_LOGIN!;"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
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
			cursor.execute("UPDATE Tbl_user SET U_code = \"" + codeFinal + "\", U_codeTime = \"" + Timestamp() + "\", U_codeAttempts = 0, U_codeType = \"ADMIN_PASSWORD_CHANGE\" WHERE U_username = \"__ADMIN__\";")
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
		Log.ServerEventLog("ADMIN_PASSWORD_CHANGE_REQUEST", details)
		htmlDetails = details.replace("\n","<br>")
		subject = SUPPORT_EMAIL_PASSWORD_CHANGE_ADMIN_SUBJECT
		text = SUPPORT_EMAIL_PASSWORD_CHANGE_ADMIN_PLAIN_TEXT % (details, codeFinal, ConvertFromSeconds(PASSWORD_CHANGE_CONFIRMATION_ADMIN_MAX_TIME))
		html = SUPPORT_EMAIL_PASSWORD_CHANGE_ADMIN_HTML_TEXT % (htmlDetails, codeFinal, ConvertFromSeconds(PASSWORD_CHANGE_CONFIRMATION_ADMIN_MAX_TIME))
		# CALL SENDMAIL
		Management.SendMail(SUPPORT_EMAIL_SENDER, SUPPORT_EMAIL_ADDRESS, subject, text, html, clientAddress)
		returnData = "INFRETtodo%eq!SEND_VERIFICATION_ADMIN_CHANGE_PASSWORD!;"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
		
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
		if not codeType == "ADMIN_PASSWORD_CHANGE":
			Handle.Error("NCES", "NO_PASSWORD_CHANGE_SCHEDULED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF PASSWORD CHANGE HAS BEEN REQUESTED IN THE FIRST PLACE
		if codeAttempts == -1:
			Handle.Error("NCES", "NO_PASSWORD_CHANGE_SCHEDULED", clientAddress, clientSocket, aesKey, True)
			return
		# CHECK IF VALIDATION CODE MATCHES PROVIDED CODE
		if not code == providedCode:
			# CHECK IF NUMBER OF WRONG CODES HAS BEEN EXCEEDED
			if codeAttempts + 1 >= MAX_CODE_ATTEMPTS_ADMIN:
				# USER TRIES TO BRUTEFORCE VALIDATION CODE
				Handle.Error("F2FA", None, clientAddress, clientSocket, aesKey, True)
				# BAN DEVICE
				Management.Ban("ip%eq!" + clientAddress.split(":")[0] + "!;duration%eq!" + str(WRONG_CODE_AUTOBAN_DURATION_ADMIN) + "!;", clientAddress, clientSocket, aesKey, True)
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
		if int(Timestamp()) - int(codeTime) > PASSWORD_CHANGE_CONFIRMATION_ADMIN_MAX_TIME:
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
		Log.ServerEventLog("ADMIN_PASSWORD_CHANGED", GetDetails(clientSocket))
		PrintSendToAdmin("SERVER ---> PASSWORD CHANGED           ---> " + clientAddress)
		returnData = "INFRETSEND_UPDATE"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
	
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
		Log.ServerEventLog("BAN_BY_ACCOUNT", username + " has been banned for " + duration + " seconds!")
		# KICK USER
		Management.Kick("mode%eq!username!;target%eq!" + username + ";!", clientAddress, clientSocket, aesKey)
	
	# ALLOWS CONNECTION TO SERVER AND SETS UP CLIENT HANDLER THREAD
	def AllowConnection(clientAddress, clientSocket):
		Log.ServerEventLog("CLIENT_CONNECTED", "IP: " + clientAddress)
		# DISPLAY IP OF CONNECTED CLIENT
		PrintSendToAdmin("SERVER ---> CONNECTED                  <--- " + clientAddress)
		# CREATE NEW THREAD FOR EACH CLIENT
		handlerThread = Thread(target = ClientHandler.Handler, args = (clientSocket, clientAddress))
		handlerThread.start()
		# ADD CLIENT DO CONNECTED CLIENTS
		Server.allClients.append([clientSocket, clientAddress, 0, None, None])
		logThread = Thread(target = Log.SetDetails, args = (clientAddress, clientSocket))
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
			cursor.execute("SELECT B_time, B_duration FROM Tbl_blacklist WHERE B_ip = \"" + ip + "\" ORDER BY B_id DESC LIMIT 1;")
			data = cursor.fetchall()
			time = data[0][0]
			duration = data[0][1]
		except IndexError:
			# INDEX OUT OF RANGE --> CLIENT IS NOT BANNED
			Management.AllowConnection(clientAddress, clientSocket)
			return
		except sqlite3.Error as e:
			# SQL ERROR --> DISALLOW CONNECTION AND SEND STATUS TO ADMIN
			Log.ServerEventLog("CONNECTION_FAILED_INTERNAL_SERVER_ERROR",e)
			PrintSendToAdmin("SERVER ---> CONNECTION DENIED: SQLE    ---> " + clientAddress)
			Network.Send(clientSocket, "ERRCONNECTION_FAILED_INTERNAL_SERVER_ERROR")
			return
		# CHECK IF ALL VARAIABLES HAVE BEEN SET
		if not time or not duration:
			Log.ServerEventLog("CONNECTION_FAILED_INTERNAL_SERVER_ERROR","N/A")
			PrintSendToAdmin("SERVER ---> CONNECTION DENIED: SQLE    ---> " + clientAddress)
			Network.Send(clientSocket, "ERRCONNECTION_FAILED_INTERNAL_SERVER_ERROR")
			return
		# CHECK IF CLIENT IS ALLOWED TO CONNECT AGAIN
		if int(time) + int(duration) > int(Timestamp()):
			PrintSendToAdmin("SERVER ---> CONNECTION DENIED: BANNED  ---> " + clientAddress)
			Network.Send(clientSocket, "ERRCONNECTION_FAILED_BANNED")
			return
		else:
			# ALL CHECKS PASSED --> ALLOW CONNECTION
			Management.AllowConnection(clientAddress, clientSocket)
	
	# BAN A CLIENT
	def Ban(command, clientAddress, clientSocket, aesKey, isSystem):
		# EXAMPLE COMMAND
		# ip%eq!ip!;duration%eq!duration_in_seconds!;
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
		Log.ServerEventLog("BAN_BY_IP", ip + " has been banned for " + duration + " seconds!")
		Management.Kick("mode%eq!ip!;target%eq!" + ip + ";!", clientAddress, clientSocket, aesKey)
		PrintSendToAdmin("SERVER ---> BANNED BY IP               ---> " + clientAddress)
		try:
			returnData = "INFRETBANNED"
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
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
			if codeAttempts + 1 >= MAX_CODE_ATTEMPTS:
				# USER TRIES TO BRUTEFORCE VALIDATION CODE
				Handle.Error("F2FA", None, clientAddress, clientSocket, aesKey, True)
				# BAN DEVICE FOR 1H
				Management.Ban("ip%eq!" + clientAddress.split(":")[0] + "!;duration%eq!" + str(WRONG_CODE_AUTOBAN_DURATION) + "!;", clientAddress, clientSocket, aesKey, True)
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
		if int(Timestamp()) - int(codeTime) > DELETE_ACCOUNT_CONFIRMATION_MAX_TIME:
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
		returnData = "INFRETACCOUNT_DELETED_SUCCESSFULLY"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
	
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
			if codeAttempts + 1 >= MAX_CODE_ATTEMPTS:
				# USER TRIES TO BRUTEFORCE VALIDATION CODE
				Handle.Error("F2FA", None, clientAddress, clientSocket, aesKey, True)
				# BAN DEVICE FOR 1H
				Management.Ban("ip%eq!" + clientAddress.split(":")[0] + "!;duration%eq!" + str(WRONG_CODE_AUTOBAN_DURATION) + "!;", clientAddress, clientSocket, aesKey, True)
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
		if int(Timestamp()) - int(codeTime) > NEW_DEVICE_CONFIRMATION_MAX_TIME:
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
			returnData = "INFERRACCOUNT_NOT_VERIFIED"
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
			return
		# ADD USER TO WHITELIST
		Server.authorizedClients.append([userID, clientSocket, username])
		Log.ClientEventLog("LOGIN_SUCCESSFUL", clientSocket)
		returnData = "INFLOGIN_SUCCESSFUL"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
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
		Log.ServerEventLog("COOKIE_REQUESTED", GetDetails(clientSocket))
		# RETURN COOKIE TO CLIENT
		PrintSendToAdmin("SERVER ---> COOKIE                     ---> " + clientAddress)
		returnData = "DTACKIcookie%eq!" + cookie + "!;"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
	
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
					cusor.execute("DELETE FROM Tbl_user WHERE U_username = " + username + ";")
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
					cursor.execute("UPDATE Tbl_user SET U_codeAttempts = " + str(codeAttempts) + " WHERE U_username = \"" + username + "\";")
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
		if int(Timestamp()) - int(codeTime) > ACCOUNT_ACTIVATION_MAX_TIME:
			Handle.Error("E2FA", None, clientAddress, clientSocket, aesKey, True)
			return
		# ALL CHECKS PASSED
		# VERIFY ACCOUNT
		try:
			cursor.execute("UPDATE Tbl_user SET U_isVerified = 1, U_codeAttempts = -1, U_lastPasswordChange = \"" + Timestamp() + "\", U_codeType = \"NONE\", U_codeResend = -1;")
		# SOMETHING SQL RELATED WENT WRONG --> THROW EXCEPTION
		except Exception as e:
			connection.rollback()
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		# ACCOUNT SUCCESSFULLY VERIFIED --> COMMIT CHANGES
		else:
			connection.commit()
			PrintSendToAdmin("SERVER ---> ACCOUNT VERIFIED           ---> " + clientAddress)
			returnData = "INFRETmsg%eq!ACCOUNT_VERIFIED!;"
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
		# FREE RESOURCES
		finally:
			connection.close()
	
	# (!!!DEPRECATED!!!) HANDLES REQUESTS FOR CHANGED MASTER PASSWORDS
	def MasterPasswordRequest(command, clientAddress, clientSocket, aesKey):
		# EXAMPLE COMMAND
		# username%eq!testuser1!;
		# username%eq!username!;
		creds = command.split(";")
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
		Log.ServerEventLog("SYNC_PASSWORD_HASH_REQUEST", GetDetails(clientSocket))
		# RETURN PASSSWORD HASH TO USER
		PrintSendToAdmin("SERVER ---> MASTERPASSWORD HASH        ---> " + clientAddress)
		returnData = "DTARETSYNPWDsalted_password_hash%eq!" + passwordHash + "!;"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
	
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
			if codeAttempts + 1 >= MAX_CODE_ATTEMPTS:
				# USER TRIES TO BRUTEFORCE VALIDATION CODE
				Handle.Error("F2FA", None, clientAddress, clientSocket, aesKey, True)
				# BAN DEVICE FOR 1H
				Management.Ban("ip%eq!" + clientAddress.split(":")[0] + "!;duration%eq!" + str(WRONG_CODE_AUTOBAN_DURATION) + "!;", clientAddress, clientSocket, aesKey, True)
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
		if int(Timestamp()) - int(codeTime) > PASSWORD_CHANGE_CONFIRMATION_MAX_TIME:
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
		Log.ClientEventLog("PASSWORD_CHANGE", clientSocket)
		# ALL UPDATED
		# INITIALIZE SYNCRONIZATION (REQUEST UPDATED DATA FROM USER)
		PrintSendToAdmin("SERVER ---> PASSWORD CHANGED           ---> " + clientAddress)
		returnData = "INFRETPASSWORD_CHANGED"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)

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
			# CREATE LOG
			Log.ClientEventLog("PASSWORD_CHANGE_REQUESTED", clientSocket)
			# FILL NEEDED INFORMATION TO SEND EMAIL
			subject = SUPPORT_EMAIL_PASSWORD_CHANGE_SUBJECT
			text = SUPPORT_EMAIL_PASSWORD_CHANGE_PLAIN_TEXT % (name, details, codeFinal, ConvertFromSeconds(PASSWORD_CHANGE_CONFIRMATION_MAX_TIME))
			html = SUPPORT_EMAIL_PASSWORD_CHANGE_HTML_TEXT % (name, htmlDetails, codeFinal, ConvertFromSeconds(PASSWORD_CHANGE_CONFIRMATION_MAX_TIME))
			# CALL SENDMAIL
			Management.SendMail(SUPPORT_EMAIL_SENDER, address, subject, text, html, clientAddress)
			returnData = "INFRETtodo%eq!SEND_VERIFICATION_CHANGE_PASSWORD!;"
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
		elif mode == "DELETE_ACCOUNT":
			# CREATE LOG
			Log.ClientEventLog("DELETE_ACCOUNT_REQUESTED", clientSocket)
			# FILL NEEDED INFORMATION TO SEND EMAIL
			subject = SUPPORT_EMAIL_DELETE_ACCOUNT_SUBJECT
			text = SUPPORT_EMAIL_DELETE_ACCOUNT_PLAIN_TEXT % (name, details, codeFinal, ConvertFromSeconds(DELETE_ACCOUNT_CONFIRMATION_MAX_TIME))
			html = SUPPORT_EMAIL_DELETE_ACCOUNT_HTML_TEXT % (name, htmlDetails, codeFinal, ConvertFromSeconds(DELETE_ACCOUNT_CONFIRMATION_MAX_TIME))
			# CALL SENDMAIL
			Management.SendMail(SUPPORT_EMAIL_SENDER, address, subject, text, html, clientAddress)
			returnData = "INFRETtodo%eq!SEND_VERIFICATION_DELETE_ACCOUNT!;"
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
		else:
			# COMMAND WAS INVALID
			Handle.Error("ICMD", "INVALID_MODE", clientAddress, clientSocket, aesKey, True)
			return

	# SENDS AN EMAIL USING GIVEN PARAMETERS
	def SendMail(From, To, subject, text, html, clientAddress):
		# CONNECT TO SMTP SERVER (SSL)
		server = smtplib.SMTP_SSL(host = SUPPORT_EMAIL_HOST, port = SUPPORT_EMAIL_SSL_PORT)
		# LOGIN
		try:
			server.login(SUPPORT_EMAIL_ADDRESS, SUPPORT_EMAIL_PASSWORD)
		except Exception as e:
			Handle.Error("MAIL", e, clientAddress, None, None, False)
			return
		message = MIMEMultipart("alternative")
		# SET EMAIL RELATED VARIABLES
		message["Subject"] = subject
		message["From"] = From
		message["To"] = To
		# READ IMAGE AS RAW BYTES
		imageFile = open(SUPPORT_EMAIL_LOGO, "rb")
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
		try:
			server.sendmail(From, To, message.as_string()) 
		except Exception as e:
			Handle.Error("MAIL", e, clientAddress, None, None, False)
			return
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
							Log.ServerEventLog("KICKED_USER_BY_IP", "kicked_user: " + target)
							kicked = True
					# CHECK IF CLIENT HAS BEEN KICKED
					if not kicked:
						# CLIENT NOT FOUND
						PrintSendToAdmin("SERVER ---> NO CLIENT FOUND            ---> " + clientAddress)
						returnData = "INFRETCLIENT_NOT_FOUND"
						# SEND DATA ENCRYPTED TO CLIENT
						Network.SendEncrypted(clientSocket, aesKey, returnData)
					# CLIENT HAS BEEN KICKED
					else:
						# CKECK IF CLIENT WAS ADMIN
						if not Server.admin == target:
							# SEND CONFIRMATION TO ADMIN
							PrintSendToAdmin("SERVER ---> CLIENT KICKED SUCCESSFULLY ---> " + clientAddress)
							returnData = "INFRETKICKED_CLIENT"
							# SEND DATA ENCRYPTED TO CLIENT
							Network.SendEncrypted(clientSocket, aesKey, returnData)
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
							Log.ServerEventLog("KICKED_USER_BY_IP_AND_PORT", "kicked_user: " + target)
							kicked = True
					# CHECK IF CLIENT HAS BEEN KICKED
					if not kicked:
						# CLIENT NOT FOUND
						PrintSendToAdmin("SERVER ---> NO CLIENT FOUND            ---> " + clientAddress)
						returnData = "INFRETCLIENT_NOT_FOUND"
						# SEND DATA ENCRYPTED TO CLIENT
						Network.SendEncrypted(clientSocket, aesKey, returnData)
					# CLIENT HAS BEEN KICKED
					else:
						# CKECK IF CLIENT WAS ADMIN
						if Server.admin == clientSocket:
							# SEND CONFIRMATION TO ADMIN
							PrintSendToAdmin("SERVER ---> CLIENT KICKED SUCCESSFULLY ---> " + clientAddress)
							returnData = "INFRETCLIENT_KICKED"
							# SEND DATA ENCRYPTED TO CLIENT
							Network.SendEncrypted(clientSocket, aesKey, returnData)
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
								Log.ServerEventLog("KICKED_USER_BY_USERNAME", "kicked_user: " + target)
								kicked = True
								break
				# CHECK IF CLIENT HAS BEEN KICKED
				if not kicked:
					# CLIENT NOT FOUND
					PrintSendToAdmin("SERVER ---> NO CLIENT FOUND            ---> " + clientAddress)
					returnData = "INFRETCLIENT_NOT_FOUND"
					# SEND DATA ENCRYPTED TO CLIENT
					Network.SendEncrypted(clientSocket, aesKey, returnData)
				# CLIENT HAS BEEN KICKED
				else:
					# CKECK IF CLIENT WAS ADMIN
					if Server.admin == clientSocket:
						# SEND CONFIRMATION TO ADMIN
						PrintSendToAdmin("SERVER ---> CLIENT KICKED SUCCESSFULLY ---> " + clientAddress)
						returnData = "INFRETCLIENT_KICKED"
						# SEND DATA ENCRYPTED TO CLIENT
						Network.SendEncrypted(clientSocket, aesKey, returnData)
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
			Log.ServerEventLog("ADMIN_LOGOUT", GetDetails(clientSocket))
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
			Network.Send(clientSocket, "FINmessage%eq!" + message + "!;")
		# SEND TCP FIN
		try:
			clientSocket.shutdown(socket.SHUT_RDWR)
		except:
			PrintSendToAdmin ("SERVER <-x- TCP RESET                  -x-> " + address)
		finally:
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
			Log.ServerEventLog("SHOW_CONNECTED_CLIENTS", GetDetails(clientSocket))
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
			# RETURN A LIST OF ALL ACCOUNTS
			Log.ServerEventLog("SHOW_ALL_ACCOUNTS", GetDetails(clientSocket))
			# CREATE A HEADER FOT THE TABLE
			header = CWHITE + FUNDERLINED + "USERNAME" + 12 * " " + " │ STATUS" + 1 * " " + " │ LAST ONLINE (Zulu Time)" + ENDF
			PrintSendToAdmin(header)
			# CREATE CONNECTION TO DATABASE
			connection = sqlite3.connect(Server.dataBase)
			# CREATE CURSOR OBJECT
			cursor = connection.cursor()
			# INITIALIZE LIST TO STORE USERS IN
			allUsers = []
			try:
				try:
					# GET ALL USERS FROM DATABASE
					cursor.execute("SELECT U_username from Tbl_user;")
					# FETCH DATA TABLE
					dataTable = cursor.fetchall()
					# ITERATE  OVER DATA TABLE AND APPEND USERS TO LIST
					for row in dataTable:
						allUsers.append(row[0])
				except Exception as e:
					# SQL ERROR
					# SEND ERROR MESSAGE
					Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
					return
				# ITERATE OVER USER LIST
				for user in allUsers:
					# INITIALIZE DEFAULT VALUES
					status = "OFFLINE"
					lastSeen = "JUST NOW"
					# CHECK IF USER IS LOGGED IN / ONLINE
					for client in Server.authorizedClients:
						if user in client:
							status = "ONLINE"
					# CHECK IF USER IS ADMIN
					if Server.admin and user == "__ADMIN__":
						status = "ONLINE"
					# IF THE USER IS OFFFLINE GET LAST ONLINE DATETIME
					if status == "OFFLINE":
						# INITIALIZE VARIABLE TO STORE ROUNDED UNIX TIMESTAMP IN
						unixTime = None
						try:
							# GET UNIX TIMESTAMP FROM DATABASE
							cursor.execute("SELECT L_datetime FROM Tbl_clientLog WHERE L_userid = (SELECT U_id from Tbl_user WHERE U_username = \"" + user + "\") AND L_event = \"LOGOUT\" ORDER BY L_id desc LIMIT 1;")
							data = cursor.fetchall()
							unixTime = data[0][0]
						except:
							# INDEX OUT OF RANGE --> USER IS PROBABLY ADMIN
							pass
						if unixTime:
							# CONVERT UNIX TIMESTAMP TO HUMAN READABLE FORMAT
							lastSeen = str(datetimealt.utcfromtimestamp(int(unixTime)).strftime("%Y-%m-%d %H:%M:%S"))
						else:
							# USER WAS NEVER ONLINE BEFORE OR SOME ERROR OCCURED
							lastSeen = "N/A"
					# ADD PADDING FOR TABLE
					status += (7 - len(status)) * " "
					user += (20 - len(user)) * " "
					# SET ONLINE / OFFLINE COLOR CODING
					if "ONLINE" in status:
						status = CGREEN + status
					else:
						status = CRED + status
					# CHECK IF USER IS ADMIN FOR HIGHLIGHTING 
					if user.replace(" ","") == "__ADMIN__":
						user = CRED + user
						lastSeen = CRED + lastSeen
					else:
						user = CCYAN + user
						lastSeen = CCYAN + lastSeen
					# CONCATENATE STRINGS TO TABLE ROW
					entry = user + CWHITE + " │ " + status + CWHITE + " │ " + lastSeen + ENDF
					# PRINT TABLE ROW
					PrintSendToAdmin(entry)
			finally:
				# FREE RESOURCES
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
		try:
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
		except Exception as e:
			Handle.Error("ICMD", e, clientAddress, clientSocket, aesKey, True)
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
		emailInUse = 0
		try:
			cursor.execute("SELECT U_id FROM Tbl_user WHERE U_email = \"" + email + "\";")
			emailInUse = len(cursor.fetchall())
		except Exception as e:
			# USER NAME ALREADY EXISTS
			connection.close()
			# SEND ERROR MESSAGE
			Handle.Error("SQLE", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			if not emailInUse == 0:
				returnData = "INFERREMAIL_ALREADY_IN_USE"
				# SEND DATA ENCRYPTED TO CLIENT
				Network.SendEncrypted(clientSocket, aesKey, returnData)
				PrintSendToAdmin("SERVER ---> EMAIL ADDRESS IN USE       ---> " + clientAddress)
				return
		try:
			# INSERT NEW USER INTO DATABASE
			cursor.execute("INSERT INTO Tbl_user (U_username,U_password,U_email,U_name,U_isVerified,U_code,U_codeTime,U_codeType,U_codeAttempts,U_lastPasswordChange,U_isBanned,U_codeResend) VALUES (\"" + username + "\",\"" + hashedPassword + "\",\"" + email + "\",\"" + nickname + "\",0,\"" + codeFinal + "\",\"" + codeTime + "\",\"ACTIVATE_ACCOUNT\",0,\"" + Timestamp() + "\",0,0);")
		except Exception as e:
			# USER NAME ALREADY EXISTS
			connection.close()
			# SEND ERROR MESSAGE
			Handle.Error("UEXT", e, clientAddress, clientSocket, aesKey, True)
			return
		else:
			# COMMIT CHANGES
			connection.commit()
		Log.ServerEventLog("REGISTER_NEW_USER", "User: " + username + "\n" + GetDetails(clientSocket))
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
		# GENERATE EMAIL
		subject = SUPPORT_EMAIL_REGISTER_SUBJECT
		text = SUPPORT_EMAIL_REGISTER_PLAIN_TEXT % (nickname, codeFinal, ConvertFromSeconds(ACCOUNT_ACTIVATION_MAX_TIME))
		html = SUPPORT_EMAIL_REGISTER_HTML_TEXT % (nickname, codeFinal, ConvertFromSeconds(ACCOUNT_ACTIVATION_MAX_TIME))
		returnData = "INFRETmsg%eq!SEND_VERIFICATION_ACTIVATE_ACCOUNT!;"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
		# SEND VERIFICATION CODE BY EMAIL
		Management.SendMail(SUPPORT_EMAIL_SENDER, email, subject, text, html, clientAddress)
		
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
				returnData = "LOGDMPSERVER\n" + finalData
				# SEND DATA ENCRYPTED TO CLIENT
				Network.SendEncrypted(clientSocket, aesKey, returnData)
				PrintSendToAdmin("SERVER ---> SERVER LOG DUMP            ---> " + clientAddress)
				# LOG DATA DUMP
				Log.ServerEventLog("SERVER_LOG_DUMP", GetDetails(clientSocket))
			elif command == "CLIENT":
				# EXECUTE QUERY
				cursor.execute("SELECT * FROM Tbl_clientLog;")
				# FETCH DATA
				data = cursor.fetchall()
				# FORMAT DATA
				finalData = str(data).replace(", ",",").replace('[','').replace(']','')
				# ENCRYPT AND SEND TO ADMIN
				returnData = "LOGDMPCLIENT\n" + finalData
				# SEND DATA ENCRYPTED TO CLIENT
				Network.SendEncrypted(clientSocket, aesKey, returnData)
				PrintSendToAdmin("SERVER ---> CLIENT LOG DUMP            ---> " + clientAddress)
				# LOG DATA DUMP
				Log.ServerEventLog("CLIENT_LOG_DUMP", GetDetails(clientSocket))
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
			returnData = "INFRETmsg%eq!ALREADY_ADMIN!;"
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
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
				cursor.execute("UPDATE Tbl_user SET U_code = \"" + codeFinal + "\", U_codeTime = \"" + timestamp + "\", U_codeType = \"ADMIN_NEW_LOGIN\", U_codeAttempts = 0 WHERE U_username = \"__ADMIN__\";")
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
			Log.ServerEventLog("ADMIN_LOGIN_FROM_NEW_DEVICE", details)
			# ADAPT FORMATTING TO WORK IN HTML
			htmlDetails = details.replace("\n","<br>")
			subject = SUPPORT_EMAIL_NEW_ADMIN_DEVICE_SUBJECT
			text = SUPPORT_EMAIL_NEW_ADMIN_DEVICE_PLAIN_TEXT % (details, codeFinal, ConvertFromSeconds(NEW_DEVICE_CONFIRMATION_ADMIN_MAX_TIME))
			html = SUPPORT_EMAIL_NEW_ADMIN_DEVICE_HTML_TEXT % (htmlDetails, codeFinal, ConvertFromSeconds(NEW_DEVICE_CONFIRMATION_ADMIN_MAX_TIME))
			returnData = "INFRETmsg%eq!SEND_VERIFICATION_ADMIN_NEW_DEVICE!;"
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
			# SEND ACCOUNT VALIDATION EMAIL
			Management.SendMail(SUPPORT_EMAIL_SENDER, SUPPORT_EMAIL_ADDRESS, subject, text, html, clientAddress)
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
		isLoggedIn = False
		for client in Server.authorizedClients:
			if clientSocket in client:
				isLoggedIn = True
		if isLoggedIn:
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
		returnData = "INFRETmsg%eq!SUCCESSFUL_ADMIN_LOGIN!;"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
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
		Log.ServerEventLog("SERVER_SHUTDOWN", GetDetails(clientSocket))
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
		Log.ServerEventLog("SERVER_REBOOT", GetDetails(clientSocket))
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
		os.system("sleep " + str(REBOOT_TIME) + " && python3 " + inspect.getfile(inspect.currentframe()))
	
	# LOGIN A CLIENT AND ADD HIM TO AUTHORIZED CLIENTS (SAVES EXPENSIVE DATABASE LOOKUPS)
	def Login(command, clientAddress, clientSocket, aesKey):
		# CHECK IF USER IS ADMIN
		if clientSocket == Server.admin:
			returnData = "You are already Admin. Use \'logout\' and try again."
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
			PrintSendToAdmin("SERVER ---> ALREADY LOGGED IN          ---> " + clientAddress)
			return
		# CHECK IF USER IS LOGGED IN ALREADY
		for client in Server.authorizedClients:
			if clientSocket in client:
				returnData = "INFRETALREADY_LOGGED_IN"
				# SEND DATA ENCRYPTED TO CLIENT
				Network.SendEncrypted(clientSocket, aesKey, returnData)
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
			Log.ClientEventLog("LOGIN_FROM_NEW_DEVICE", clientSocket)
			# ADAPT FORMATTING TO WORK IN HTML
			htmlDetails = details.replace("\n","<br>")
			subject = SUPPORT_EMAIL_NEW_DEVICE_SUBJECT
			text = SUPPORT_EMAIL_NEW_DEVICE_PLAIN_TEXT % (name, details, codeFinal, ConvertFromSeconds(NEW_DEVICE_CONFIRMATION_MAX_TIME))
			html = SUPPORT_EMAIL_NEW_DEVICE_HTML_TEXT % (name, htmlDetails, codeFinal, ConvertFromSeconds(NEW_DEVICE_CONFIRMATION_MAX_TIME))
			returnData = "INFRETmsg%eq!SEND_VERIFICATION_NEW_DEVICE!;"
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
			# SEND ACCOUNT VALIDATION EMAIL
			Management.SendMail(SUPPORT_EMAIL_SENDER, address, subject, text, html, clientAddress)
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
			returnData = "INFERRACCOUNT_NOT_VERIFIED"
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
			return
		# ADD USER TO WHITELIST
		Server.authorizedClients.append([userID, clientSocket, username])
		Log.ClientEventLog("LOGIN_SUCCESSFUL", clientSocket)
		returnData = "INFRETLOGIN_SUCCESSFUL"
		# SEND DATA ENCRYPTED TO CLIENT
		Network.SendEncrypted(clientSocket, aesKey, returnData)
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
			Log.ServerEventLog("ADMIN_LOGOUT", GetDetails(clientSocket))
			Server.admin = None
			Server.adminIp = None
			isLoggedout = True
			print("SERVER **** ADMIN LOGOUT               **** " + clientAddress)
			if not isDisconnected:
				returnData = "INFRETLOGGED_OUT"
				# SEND DATA ENCRYPTED TO CLIENT
				Network.SendEncrypted(clientSocket, aesKey, returnData)
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
							returnData = "INFRETLOGGED_OUT"
							# SEND DATA ENCRYPTED TO CLIENT
							Network.SendEncrypted(clientSocket, aesKey, returnData)
						break
			# USER IS NOT WHITELISTED
			if not isLoggedout and not isDisconnected:
				try:
					# RETURN ERROR MESSAGE TO CLIENT
					returnData = "INFERRNOT_LOGGED_IN"
					# SEND DATA ENCRYPTED TO CLIENT
					Network.SendEncrypted(clientSocket, aesKey, returnData)
				except:
					Network.Send(clientSocket, "INFERRNOT_LOGGED_IN")
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
			returnData = "INFERRNOT_LOGGED_IN"
			# SEND DATA ENCRYPTED TO CLIENT
			Network.SendEncrypted(clientSocket, aesKey, returnData)
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
		text = "INFMIRmsg%eq!" + text + "!;"
		encryptedData = aesEncryptor.encrypt(text)
		hmacKeys = GetHMACkeys(Server.admin)
		try:
			Server.admin.send(b'\x01' + bytes("E" + encryptedData + CryptoHelper.CalculateHMAC(hmacKeys[0], hmacKeys[1], encryptedData), "utf-8") + b'\x04')
		except SocketError as e:
			Log.ServerEventLog("SOCKET_ERROR", str(e))
			Management.Logout(Server.adminIp, Server.admin, Server.adminAesKey, True)

# GENERATES A 2 FACTOR AUTHENTICATION CODE
def CodeGenerator():
	code = str(secrets.randbelow(1000000))
	while not len(code) >= 6:
		code = "0" + code
	return "PM-" + code

# RETURNS KNOWN DETAILS RELATED TO A CONNECTED CLIENT
def GetDetails(clientSocket):
	for client in Server.allClients:
		if clientSocket in client:
			return client[3]
	return "N/A"
	
def ConvertFromSeconds(_seconds):
	return str(datetime.timedelta(seconds=_seconds))
	
def GetHMACkeys(clientSocket):
	hmacKey = None
	for client in Server.allClients:
		if clientSocket in client:
			hmacKey = client[4]
			break
	if not hmacKey:
		return False
	else:
		return [hmacKey[:32], hmacKey[32:]]
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
	allClients = [] #[[socket, address, adminFlag, details, HMACkey],[...]]
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
		
		# RUN CHECKS
		print(CWHITE + "         Initializing boot sequence ..." + ENDF)
		print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Boot sequence initialized." + ENDF)
		print(CWHITE + "         Checking python version ..." + ENDF)
		if not sys.version.split(" ")[0] in PYTHON_VERSIONS:
			print(CWHITE + "[" + CYELLOW + "WARNING" + CWHITE + "] Not tested on python " + sys.version.split(" ")[0] + ENDF)
		else:
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Current python version: " + sys.version.split(" ")[0] + ENDF)
		print(CWHITE + "         Checking config ..." + ENDF)
		print(CWHITE + "         Checking global variables ..." + ENDF)
		try:
			if not REBOOT_TIME or not LOCAL_ADDRESS or LOCAL_ADDRESS == "" or not LOCAL_PORT or not SUPPORT_EMAIL_HOST or SUPPORT_EMAIL_HOST == "" or not SUPPORT_EMAIL_SSL_PORT or not SUPPORT_EMAIL_ADDRESS or SUPPORT_EMAIL_ADDRESS == "" or not SUPPORT_EMAIL_PASSWORD or SUPPORT_EMAIL_PASSWORD == "" or not ACCOUNT_ACTIVATION_MAX_TIME or not DELETE_ACCOUNT_CONFIRMATION_MAX_TIME or not NEW_DEVICE_CONFIRMATION_MAX_TIME or not NEW_DEVICE_CONFIRMATION_ADMIN_MAX_TIME or not PASSWORD_CHANGE_CONFIRMATION_MAX_TIME or not PASSWORD_CHANGE_CONFIRMATION_ADMIN_MAX_TIME or not CONFIG_VERSION or not CONFIG_BUILD or not MAX_CODE_ATTEMPTS or not MAX_CODE_ATTEMPTS_ADMIN or WRONG_CODE_AUTOBAN_DURATION == None or WRONG_CODE_AUTOBAN_DURATION_ADMIN == None or RESEND_CODE_MAX_COUNT == None or not SUPPORT_EMAIL_DELETE_ACCOUNT_SUBJECT or not SUPPORT_EMAIL_DELETE_ACCOUNT_PLAIN_TEXT or not SUPPORT_EMAIL_DELETE_ACCOUNT_HTML_TEXT or not SUPPORT_EMAIL_NEW_DEVICE_SUBJECT or not SUPPORT_EMAIL_NEW_DEVICE_PLAIN_TEXT or not SUPPORT_EMAIL_NEW_DEVICE_HTML_TEXT or not SUPPORT_EMAIL_PASSWORD_CHANGE_SUBJECT or not SUPPORT_EMAIL_PASSWORD_CHANGE_PLAIN_TEXT or not SUPPORT_EMAIL_PASSWORD_CHANGE_HTML_TEXT or not SUPPORT_EMAIL_REGISTER_SUBJECT or not SUPPORT_EMAIL_REGISTER_PLAIN_TEXT or not SUPPORT_EMAIL_REGISTER_HTML_TEXT or not SUPPORT_EMAIL_NEW_ADMIN_DEVICE_SUBJECT or not SUPPORT_EMAIL_NEW_ADMIN_DEVICE_PLAIN_TEXT or not SUPPORT_EMAIL_NEW_ADMIN_DEVICE_HTML_TEXT or not SUPPORT_EMAIL_PASSWORD_CHANGE_ADMIN_SUBJECT or not SUPPORT_EMAIL_PASSWORD_CHANGE_ADMIN_PLAIN_TEXT or not SUPPORT_EMAIL_PASSWORD_CHANGE_ADMIN_HTML_TEXT or USE_PERSISTENT_RSA_KEYS == None or not PYTHON_VERSIONS:
				if CONFIG_VERSION and not CONFIG_VERSION == VERSION:
					print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] FATAL: Server is on version " + VERSION + " but config file is for version " + CONFIG_VERSION + "." + ENDF)
				else:
					print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] FATAL: Undefined variables in config file." + ENDF)
				return
			else:
				print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Checked global variables. " + ENDF)
		except:
			try:
				print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] FATAL: Server is on version " + VERSION + " but config file is for version " + CONFIG_VERSION + "." + ENDF)
			except NameError:
				print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] FATAL: Invalid config file. Please download the config file for PMDBServer version " + VERSION + " ." + ENDF)
			return
		else:
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Config is valid. " + ENDF)
		print(CWHITE + "         Checking version info ..." + ENDF)
		if not CONFIG_VERSION == VERSION:
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] FATAL: Server is on version " + VERSION + " but config file is for version " + CONFIG_VERSION + "." + ENDF)
			return
		else:
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Checked version info. Current version: " + VERSION + ". " + ENDF)
		print(CWHITE + "         Checking build info ..." + ENDF)
		if not CONFIG_BUILD == BUILD:
			print(CWHITE + "[" + CYELLOW + "WARNING" + CWHITE + "] Server is on " + BUILD + "-build but config file is for " + CONFIG_BUILD + "-build." + ENDF)
		else:
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Checked build info. Current build: " + BUILD + "-build. " + ENDF)
		print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Checked config. " + ENDF)
		PYTHON = "Python " + " / ".join(PYTHON_VERSIONS) + " - LINUX"
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
					adminPassword = CryptoHelper.SHA256(newAdminPassword)
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
		print(CWHITE + "         Running garbage collection on database ..." + ENDF)
		if not DatabaseManagement.GarbageCollection():
			print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Garbage collection. " + ENDF)
		else:
			print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Deleted deprecated data." + ENDF)
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
		if USE_PERSISTENT_RSA_KEYS:
			rsaInitialized = False
			while not rsaInitialized:
				print(CWHITE + "         Checking for RSA private key file in " + os.getcwd() + "/keys ..." + ENDF)
				if not os.path.isdir("keys"):
					print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Directory \"keys\" does not exist in " + os.getcwd() + "." + ENDF)
					print(CWHITE + "         Creating directory \"keys\" ..." + ENDF)
					os.mkdir("keys")
					if os.path.isdir("keys"):
						print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Directory \"keys\" created successfully." + ENDF)
					else:
						print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Fatal: could not create doirectory \"keys\ in " + os.getcwd() + "." + ENDF)
						exit()
				keyFiles = glob.glob(os.getcwd() + "/keys/*.privatekey")
				if len(keyFiles) == 0:
					selected = False
					while not selected:
						print(CWHITE + "         No key file found!" + ENDF)
						print(CWHITE + "[" + CYELLOW + "MANUAL" + CWHITE + "] What to do next?" + ENDF)
						print(CWHITE + "         [R] = Retry" + ENDF)
						print(CWHITE + "         [G] = Generate new RSA Keys" + ENDF)
						selectedOption = input(CWHITE + " > ")
						if selectedOption.upper() == "G":
							print(CWHITE + "         Generating RSA keys ..." + ENDF)
							keyPair = CryptoHelper.RSAKeyPairGenerator()
							print(CWHITE + "         Exporting private RSA key ..." + ENDF)
							privateKey = keyPair[1].exportKey(format="PEM").decode("utf-8")
							with open("keys/server.privatekey", "w") as out:
								out.write(privateKey)
								out.close()
							print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Private RSA key exported successfully." + ENDF)
							print(CWHITE + "         Exporting public RSA key ..." + ENDF)
							publicKey = keyPair[0].exportKey(format="PEM").decode("utf-8")
							with open("keys/server.publicKey", "w") as out:
								out.write(publicKey)
								out.close()
							print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Public RSA key exported successfully." + ENDF)
							print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] RSA key pair generated successfully." + ENDF)
							print(CWHITE + "[ " + CCYAN + "INFO" + CWHITE + " ] You can find the new keys in the \"keys\" folder located in " + os.getcwd() + " ..." + ENDF)     
							selected = True
						elif selectedOption.upper() == "R":
							selected = True
						else:
							print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Invalid option! Please try again. " + ENDF)
				elif len(keyFiles) == 1:
					# GLOB RETURNS NAME + PATH --> REMOVE PATH
					pathParts = keyFiles[0].split("/")
					privateKey = pathParts[len(pathParts) - 1]
					print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Found RSA private key \"" + privateKey + "\" in " + os.getcwd() + ENDF)
					print(CWHITE + "         Autoselecting ..." + ENDF)
					print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Selected \"" + privateKey + "\" ." + ENDF)
					print(CWHITE + "         Checking for READ permission ..." + ENDF)
					if os.access(keyFiles[0], os.R_OK):
						print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Checked for READ permission." + ENDF)
					else:
						print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Checked for READ permission." + ENDF)
						print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] FATAL: Insufficient permissions!" + ENDF)
						return
					print(CWHITE + "         Reading private key ..." + ENDF)
					with open(keyFiles[0], "rb") as f:
						try:
							rsaTmp = RSA.importKey(f.read())
							Server.serverPrivateKey = rsaTmp
							Server.serverPublicKey = rsaTmp.publickey()
							Server.publicKeyPem = Server.serverPublicKey.exportKey(format="PEM").decode("utf-8")
							Server.publicKeyXml = CryptoHelper.RSAPublicPemToXml(Server.publicKeyPem)
						except Exception as e:
							print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Invalid RSA key. Exception: " + str(e) + ENDF)
						else:
							print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] RSA Keys successfully set up." + ENDF)
						f.close()
					rsaInitialized = True
				else:
					selected = False
					# LOOP IN CASE USER INPUT IS INVALID
					while not selected:
						iterator = 0
						print(CWHITE + "         Found more than private key in " + os.getcwd() + "/keys" + ENDF)
						print(CWHITE + "[" + CYELLOW + "MANUAL" + CWHITE + "] Which private key do you want to use? (enter index)" + ENDF)
						cleanedPrivateKeys = []
						# LIST AVAILABLE DATABASES
						for keypath in keyFiles:
							pathParts = keypath.split("/")
							key = pathParts[len(pathParts) - 1]
							print(CWHITE + "         [" + str(iterator) + "] " + key + ENDF)
							cleanedPrivateKeys.append(key)
							iterator += 1
						# PROMPT USER INPUT
						selectedKeyString = input(CWHITE + " > ")
						# TRY TO SELECT DATABASE AT SELECTED INDEX
						try:
							selectedKey = int(selectedKeyString)
							pathParts = keyFiles[selectedKey].split("/")
							privateKey = pathParts[len(pathParts) - 1]
							print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Selected \"" + privateKey + "\" ." + ENDF)
							print(CWHITE + "         Checking for READ permission ..." + ENDF)
							if os.access(keyFiles[selectedKey], os.R_OK):
								print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] Checked for READ permission." + ENDF)
							else:
								print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Checked for READ permission." + ENDF)
								print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] FATAL: Insufficient permissions!" + ENDF)
								return
							with open(keyFiles[selectedKey], "rb") as f:
								try:
									rsaTmp = RSA.importKey(f.read())
									Server.serverPrivateKey = rsaTmp
									Server.serverPublicKey = rsaTmp.publickey()
									Server.publicKeyPem = Server.serverPublicKey.exportKey(format="PEM").decode("utf-8")
									Server.publicKeyXml = CryptoHelper.RSAPublicPemToXml(Server.publicKeyPem)
								except Exception as e:
									print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Invalid RSA key. Exception: " + e + ENDF)
								else:
									print(CWHITE + "[  " + CGREEN + "OK" + CWHITE + "  ] RSA Keys successfully set up." + ENDF)
									selected = True
									rsaInitialized = True
								f.close()
						# INDEX WAS INVALID --> RETRY
						except:
							print(CWHITE + "[" + CRED + "FAILED" + CWHITE + "] Invalid selection! Retrying ..." + ENDF)
		else:
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
# ---> VIEW DOCUMENTATION
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
		nonce = None
		clientAddress = address
		isSocketError = False
		isDisconnected = False
		isTimedOut = False
		isTcpFin = False
		isXmlClient = False
		keyExchangeFinished = False
		message = ""
		# AWAIT PACKETS FROM CLIENT
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
					message = "RECEIVED_TCP_FIN"
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
							message = "RECEIVED_FIN"
							return
						elif packetID == "INI":
							PrintSendToAdmin("SERVER <--- CLIENT HELLO               <--- " + clientAddress)
							# GET PACKET ID
							packetSID = dataString[4:7]
							# GET FORMAT
							if packetSID == "XML":
								isXmlClient = True
								# SEND PUBLIC KEY
								Network.Send(clientSocket, "KEYXMLkey%eq!" + Server.publicKeyXml + "!;")
							elif packetSID == "PEM":
								isXmlClient = False
								# SEND PUBLIC KEY
								Network.Send(clientSocket, "KEYPEMkey%eq!" + Server.publicKeyPem + "!;")
							else:
								PrintSendToAdmin("SERVER <-#- [ERRNO 02] IRSA            -#-> " + clientAddress)
								message = "SECURITY_EXCEPTION_INVALID_RSA_KEY"
								return
							PrintSendToAdmin("SERVER ---> SERVER HELLO               ---> " + clientAddress)
						# CHECK IF PACKET ID IS KNOWN BUT USED IN WRONG CONTEXT
						elif packetID in ("EXC", "DTA", "INF", "REQ", "MNG", "LOG", "KEX"):
							# RECEIVED SOME OTHER PACKET OVER UNENCRYPTED CONNECTION
							# UNSECURE CONNECTION
							PrintSendToAdmin("SERVER <-#- [ERRNO 03] USEC             -#-> " + clientAddress)
							# JUMP TO FINALLY AND FINISH CONNECTION
							message = "SECURITY_EXCEPTION_RECEIVED_SECURE_PACKET_OVER_UNSECURE_CONNECTION"
							return
						else:
							# RECEIVED INVALID PACKET ID
							PrintSendToAdmin("SERVER <-#- [ERRNO 04] IPID            -#-> " + clientAddress)
							# JUMP TO FINALLY AND FINISH CONNECTION
							message = "GENERIC_EXCEPTION_INVALID_PACKET_ID"
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
							cryptNonce = None
							# EXTRACT KEY AND NONCE FROM PACKET
							try:
								for info in cryptoInformation:
									if "key" in info:
										key = info.split("!")[1]
									elif "nonce" in info:
										cryptNonce = info.split("!")[1]
									elif len(info) == 0:
										pass
									else:
										# COMMAND CONTAINED MORE INFORMATION THAN REQUESTED
										Handle.Error("ICMD", "TOO_MANY_ARGUMENTS", clientAddress, clientSocket, None, False)
										message = "GENERIC_EXCEPTION_TOO_MANY_ARGUMENTS"
										return
							except:
								# COMMAND HAS UNKNOWN FORMATTING
								Handle.Error("ICMD", "INVALID_FORMATTING", clientAddress, clientSocket, None, False)
								message = "GENERIC_EXCEPTION_INVALID_FORMATTING"
								return
							# COMMAND DID NOT CONTAIN ALL INFORMATION
							if not cryptNonce or not key:
								Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, None, False)
								message = "GENERIC_EXCEPTION_TOO_FEW_ARGUMENTS"
								return
							try:
								if isXmlClient:
									clientPublicKey = CryptoHelper.RSAPublicXmlToPem(key)
								else:
									clientPublicKey = key
							except:
								Handle.Error("IRSA", "INVALID_RSA_KEY", clientAddress, clientSocket, None, False)
								message = "SECURITY_EXCEPTION_INVALID_RSA_KEY"
								return
							# GENERATE 256 BIT AES KEY
							aesKey = CryptoHelper.AESKeyGenerator()
							# PrintSendToAdmin("AES: " + aesKey)
							# ENCRYPT AES KEY USING RSA 4096
							decNonce = CryptoHelper.RSADecrypt(cryptNonce, Server.serverPrivateKey)
							CryptoHelper.GenerateHMACkey(aesKey, decNonce, clientSocket)
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
							message = "GENERIC_EXCEPTION_INVALID_PACKET_ID"
							return
					# CHECK IF PACKET IS AES ENCRYPTED
					elif packetSpecifier == 'E':
						# CREATE AES CIPHER
						hmacKeys = GetHMACkeys(clientSocket)
						if not CryptoHelper.VerifyHMAC(hmacKeys[0], hmacKeys[1], dataString[1:]):
							Handle.Error("IMAC", None, clientAddress, clientSocket, None, False)
							message = "SECURITY_EXCEPTION_INVALID_HMAC_CHECKSUM"
							return
						aesDecryptor = AESCipher(aesKey)
						# DECRYPT DATA
						decryptedData = aesDecryptor.decrypt(dataString[1:-44])
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
									# TOO FEW ARGUMENTS
									Handle.Error("ICMD", "TOO_FEW_ARGUMENTS", clientAddress, clientSocket, None, False)
									message = "GENERIC_EXCEPTION_TOO_FEW_ARGUMENTS"
									return
								# ENCRYPT DATA
								returnData = "KEXACKnonce%eq!" + nonce + "!;"
								# SEND DATA ENCRYPTED TO CLIENT
								Network.SendEncrypted(clientSocket, aesKey, returnData)
								PrintSendToAdmin("SERVER <--- ACKNOWLEDGE                ---> " + clientAddress)
								PrintSendToAdmin("SERVER <--- KEY EXCHANGE FINISHED      ---> " + clientAddress)
								keyExchangeFinished = True
							else:
								PrintSendToAdmin("SERVER <-#- [ERRNO 06] ISID             -#-> " + clientAddress)
								# JUMP TO FINALLY AND FINISH CONNECTION
								message = "GENERIC_EXCEPTION_INVALID_SUB_ID"
								return
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
								message = "GENERIC_EXCEPTION_INVALID_SUB_ID"
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
							# INITIALIZE PASSWORD CHANGE
							elif packetSID == "IPC":
								PrintSendToAdmin("SERVER <--- INITIALIZE PASSWORD CHANGE <--- " + clientAddress)
								mgmtThread = Thread(target = Management.AccountRequest, args = (decryptedData[9:], clientAddress, clientSocket, aesKey))
								mgmtThread.start()
							# INITAILIZE ACCOUNT DELETION
							elif packetSID == "IAD":
								PrintSendToAdmin("SERVER <--- INITIALIZE DELETE ACCOUNT  <--- " + clientAddress)
								mgmtThread = Thread(target = Management.AccountRequest, args = (decryptedData[9:], clientAddress, clientSocket, aesKey))
								mgmtThread.start()
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
							elif packetSID == "CAD":
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
							# REQUEST MASTER-PASSWORD HASH --> DANGEROUS!!! use CredentialCheckProvider() INSTEAD
							#elif packetSID == "PWH":
							#	PrintSendToAdmin("SERVER <--- REQUEST PASSWORD HASH      <--- " + clientAddress)
							#	mgmtThread = Thread(target = Management.MasterPasswordRequest, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
							#	mgmtThread.start()
							# CHANGE EMAIL ADDRESS (ONLY IF ACCOUNT IS NOT YET ACTIVATED)
							elif packetSID == "CEA":
								PrintSendToAdmin("SERVER <--- CHANGE EMAIL ADDRESS       <--- " + clientAddress)
								mgmtThread = Thread(target = Management.ChangeEmailAddress, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
								mgmtThread.start()
							# CHECK IF COOKIE EXISTS
							elif packetSID == "CCK":
								PrintSendToAdmin("SERVER <--- CHECK COOKIE               <--- " + clientAddress)
								mgmtThread = Thread(target = Management.CheckCookie, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
								mgmtThread.start()
							# GET ACCOUNT ACTIVITY
							elif packetSID == "GAA":
								PrintSendToAdmin("SERVER <--- REQUEST ACCOUNT ACTIVITY   <--- " + clientAddress)
								mgmtThread = Thread(target = Management.GetAccountActivity, args = (clientAddress, clientSocket, aesKey))
								mgmtThread.start()
							# RESEND 2FA CODE
							elif packetSID == "RTC":
								PrintSendToAdmin("SERVER <--- RESEND 2FA CODE            <--- " + clientAddress)
								mgmtThread = Thread(target = Management.ResendCode, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
								mgmtThread.start()
							# CHANGE NAME OF USER USED IN MESSAGES
							elif packetSID == "CHN":
								PrintSendToAdmin("SERVER <--- REQUEST NAME CHANGE        <--- " + clientAddress)
								mgmtThread = Thread(target = Management.ChangeName, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
								mgmtThread.start()
							# CHECK CREDENTIALS IN CASE OF PASSWORD CHANGES
							elif packetSID == "CCR":
								PrintSendToAdmin("SERVER <--- CREDENTIAL CHECK REQUEST   <--- " + clientAddress)
								mgmtThread = Thread(target = Management.CredentialCheckProvider, args = (decryptedData[6:], clientAddress, clientSocket, aesKey))
								mgmtThread.start()
							else:
								PrintSendToAdmin("SERVER <-#- [ERRNO 06] ISID             -#-> " + clientAddress)
								# JUMP TO FINALLY AND FINISH CONNECTION
								message = "GENERIC_EXCEPTION_INVALID_SUB_ID"
								return
					else:
						# INVALID PACKET SPECIFIER
						PrintSendToAdmin("SERVER <-#- [ERRNO 05] IPSP             -#-> " + clientAddress)
						message = "GENERIC_EXCEPTION_INVALID_PACKET_SPECIFIER"
						return
				else:
					return

		
# INITIALIZE THE SERVER
ServerThread = Server()
ServerThread.start()
