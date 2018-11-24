#!/usr/bin/python

import sys
import socket
import Crypto
from Crypto.PublicKey import RSA
from Crypto import Random
import ast
from threading import Thread
from Crypto.Util import number
from Crypto.Util.asn1 import DerSequence
from base64 import standard_b64encode, b64decode
from binascii import a2b_base64
from os.path import basename, exists
from xml.dom import minidom
from Crypto.Cipher import PKCS1_OAEP
import base64
import argparse
import secrets
import readline
import hashlib
import time
from socket import error as SocketError
import os
from Crypto import Random
from Crypto.Cipher import AES
################################################################################
#------------------------------CLIENT CRYPTO CLASS-----------------------------#
################################################################################

# PROVIDES CRYPTOGRAPHIC METHODS
class CryptoHelper():

	# RETURNS SHA256 HASH OF PLAINTEXT
	def SHA256(plaintext):
		hashBytes = base64.b64encode(hashlib.sha256(bytes(plaintext, "utf-8")).digest())
		return hashBytes.decode("utf-8")
	
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
		
	def RSADecrypt(cryptotext, privateKey):
		cipher = PKCS1_OAEP.new(privateKey)
		decryptedBytes = base64.b64decode(bytes(cryptotext,"utf-8"))
		decryptedBytes = cipher.decrypt(decryptedBytes)
		return decryptedBytes.decode("utf-8")
		
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
		
		

	
class IO(Thread):
	publicKey = None
	privateKey = None
	pemKey = None
	def run(self):
		keyPair = CryptoHelper.RSAKeyPairGenerator()
		IO.publicKey = keyPair[0]
		IO.privateKey = keyPair[1]
		IO.pemKey = IO.publicKey.exportKey(format="PEM").decode("utf-8")
		ACThread = ActiveConnection()
		ACThread.start()
		try:
			while True:
				parameters = input("").split("-")
				command = parameters[0].replace(" ","")
				if command == "pwchange":
					self.IOpasswordChangeRequest(parameters)
				elif command == "exit":
					self.IOexit(parameters)
				elif command == 'register':
					self.IOregister(parameters)
				elif command == 'insert':
					self.IOinsert(parameters)
				elif command == 'select':
					self.IOselect(parameters)
				elif command == 'update':
					self.IOupdate(parameters)
				elif command == 'msg':
					self.IOmsg(parameters)
				elif command == 'sync':
					self.IOsync(parameters)
				elif command == 'alldata':
					self.IOall(parameters)
				elif command == 'login':
					self.IOlogin(parameters)
				elif command == "logout":
					self.IOlogout(parameters)
				elif command == 'su':
					self.IOsudo(parameters)
				elif command == 'shutdown':
					self.IOshutdown(parameters)
				elif command == 'reboot':
					self.IOreboot(parameters)
				elif command == 'start':
					self.IOstart(parameters)
				elif command == "serverlog":
					self.IOslog(parameters)
				elif command == "clientlog":
					self.IOclog(parameters)
				elif command == "listallclients":
					self.IOlistAllClients(parameters)
				elif command == "error":
					self.IOerror(parameters)
				elif command == "cookie":
					self.IOcookie(parameters)
				elif command == "kick":
					self.IOkick(parameters)
				elif command == "verify":
					self.IOverify(parameters)
				elif command == "confirmnewdevice":
					self.IOconfirmNewDevice(parameters)
				elif command == "newadmindevice":
					self.IOnewAdminDevice(parameters)
				elif command == "initadminpwchange":
					self.IOinitAdminPwChange(parameters)
				elif command == "commitadminpwchange":
					self.IOcommitAdminPwChange(parameters)
				elif command == "initpwchange":
					self.IOinitPwChange(parameters)
				elif command == "commitpwchange":
					self.IOcommitPwChange(parameters)
				elif command == "initdelaccount":
					self.IOinitDelAccount(parameters)
				elif command == "commitdelaccount":
					self.IOcommitDelAccount(parameters)
				elif command == "banclient":
					self.IObanClient(parameters)
				elif command == "banaccount":
					self.IObanAccount(parameters)
				elif command == "listallusers":
					self.IOListAllUsers(parameters)
				elif command == "help":
					self.IOhelp(parameters)
				elif command == "getaccountactivity":
					self.IOgetAccountActivity(parameters)
				elif command == "changeemailaddress":
					self.IOchangeEmailAddress(parameters)
				elif command == "resendcode":
					self.IOresendCode(parameters)
				elif command == "changename":
					self.IOchangeName(parameters)
				elif command == "enabledebugging":
					self.IOenableDebugging(parameters)
				elif command == "disabledebugging":
					self.IOdisableDebugging(parameters)
				elif command == "masterpwrequest":
					self.IOmasterPasswordRequest(parameters)
				elif command == "delete":
					self.IOdelete(parameters)
				else:
					print('unknown command ' + '\'' + command + '\'')
		except Exception as e:
			print(e)
			pass
			
	def IOhelp(self, parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		allCommands = """
		
	AVAILABLE COMMANDS:\n
	alldata
	banaccount
	banclient
	changeemailaddress
	changename
	clientlog
	commitadminpwchange
	commitdelaccount
	commitpwchange
	confirmnewdevice
	cookie
	delete
	error
	exit
	getaccountactivity
	initadminpwchange
	initdelaccount
	initpwchange
	insert
	kick
	listallclients
	listallusers
	login
	logout
	masterpwrequest
	msg
	newadmindevice
	pwchange
	reboot
	register
	resendcode
	select
	serverlog
	shutdown
	start
	su
	sync
	update
	verify
	
	help
	
	Type \"<command> --help\" for more information about commands
		"""
		print(allCommands)
	
	def IOenableDebugging(self, parameters):
		try:
			if parameters[1] == "h":
				print("enable debugging output.\nthis command takes no arguments.")
				return
		except Exception as e:
			print(e)
		ActiveConnection.debugging = True
		
	def IOdisableDebugging(self, parameters):
		try:
			if parameters[1] == "h":
				print("disable debugging output.\nthis command takes no arguments.")
				return
		except Exception as e:
			print(e)
		ActiveConnection.debugging = False
		
		
		
	def IOkick(self, parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\nkick\n-m <ip/ipport/username>\n-t <target>")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			mode = None
			target = None
			for command in parameters:
				if command.split(" ")[0] == "m":
					mode = command.split(" ")[1]
				elif command.split(" ")[0] == "t":
					target = command.split(" ")[1]
				elif len(command) == 0 or command == "kick ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not mode or not target:
				print("too few arguments")
				return
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGKIKmode%eq!" + mode + "!;target%eq!" + target + "!")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			
			print('typo in command ' + '\'' + command + '\'')
	
	def IOpasswordChangeRequest(self, parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGIPCmode%eq!PASSWORD_CHANGE!;")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
	
	def IOlistAllClients(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLICmode%eq!ALL_CONNECTED!;")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
	
	def IOListAllUsers(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLICmode%eq!ALL_USERS!;")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
	
	def IOslog(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLOGSERVER")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
	
	def IOclog(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLOGCLIENT")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04') 
	   
	def IOauth():
		ActiveConnection.clientSocket.send(b'\x01' + bytes("UINIPEM", "utf-8") + b'\x04')
		
	def IOexit(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		ActiveConnection.clientSocket.send(b'\x01' + bytes("UFIN", "utf-8") + b'\x04')
		exit()
		
	def IOcookie(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGCKI")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
	
	def IOmsg(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		ActiveConnection.clientSocket.send(b'\x01' + bytes("UHello", "utf-8") + b'\x04')
		
	def IOregister(self, parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\n-u <username>\n-p <password>\n-e <email>\n-n <nickname>")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			nickname = None
			email = None
			password = None
			username = None
			for command in parameters:
				if command.split(" ")[0] == "u":
					username = command.split(" ")[1]
				elif command.split(" ")[0] == "p":
					password = command.split(" ")[1]
				elif command.split(" ")[0] == "e":
					email = command.split(" ")[1]
				elif command.split(" ")[0] == "n":
					nickname = command.split(" ")[1]
				elif len(command) == 0 or command == "register ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not password or not username or not email:
				print("too few arguments")
				return
			if not nickname:
				nickname = "user"
			finalPassword = CryptoHelper.SHA256(CryptoHelper.SHA256(password)[:32])
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGREGusername%eq!" + username + "!;password%eq!" + finalPassword + "!;email%eq!" + email + "!;nickname%eq!" + nickname + "!;cookie%eq!" + ActiveConnection.cookie + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + "-".join(parameters) + "\"")
		
	def IOinsert(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\ninsert\n-hs <host>\n-p <password>\n\nOPTIONAL:\n-u <username>\n-e <email>\n-n <notes>\n-url <url>")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			host = None
			password = None
			username = None
			email = None
			notes = None
			url = None
			for command in parameters:
				if command.split(" ")[0] == "hs":
					host = command.split(" ")[1]
				elif command.split(" ")[0] == "p":
					password = command.split(" ")[1]
				elif command.split(" ")[0] == "u":
					username = command.split(" ")[1]
				elif command.split(" ")[0] == "e":
					email = command.split(" ")[1]
				elif command.split(" ")[0] == "n":
					notes = command.split(" ")[1]
				elif command.split(" ")[0] == "url":
					url = command.split(" ")[1]
				elif len(command) == 0 or command == "insert ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not host or not password:
				print("too few arguments")
				return
			query = "local_id%eq!" + str(secrets.randbelow(10**6)) + "!;host%eq!" + host + "!;password%eq!" + password + "!;"
			if username:
				query += "uname%eq!" + username + "!;"
			if email:
				query += "email%eq!" + email + "!;"
			if notes:
				query += "notes%eq!" + notes + "!;"
			if url:
				query += "url%eq!" + url + "!;"
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("REQINS" + query + "datetime%eq!" + str(time.time()).split('.')[0] + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
		
	def IOselect(self,parameters):
		try:
			if parameters[1] == "h":
				print("select -HID1 HID2 HID3 [...]")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		hidString = ""
		hids = parameters[1].split(" ")
		for HID in hids:
			hidString += HID + ";"
		if hidString == "":
			print("too few arguments")
			return
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("REQSEL" + hidString)
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOdelete(self,parameters):
		try:
			if parameters[1] == "h":
				print("delete -HID1 HID2 HID3 [...]")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		hidString = ""
		hids = parameters[1].split(" ")
		for HID in hids:
			hidString += HID + ";"
		if hidString == "":
			print("too few arguments")
			return
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("REQDEL" + hidString)
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOupdate(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\nupdate\n-hid <hid>\n\nOPTIONAL:\n-hs <host>\n-u <username>\n-p <password>\n-url <url>\n-n <notes> \n-e <email>")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			hid = None
			host = None
			password = None
			username = None
			email = None
			notes = None
			url = None
			for command in parameters:
				if command.split(" ")[0] == "hs":
					username = command.split(" ")[1]
				elif command.split(" ")[0] == "p":
					password = command.split(" ")[1]
				elif command.split(" ")[0] == "u":
					email = command.split(" ")[1]
				elif command.split(" ")[0] == "e":
					email = command.split(" ")[1]
				elif command.split(" ")[0] == "n":
					notes = command.split(" ")[1]
				elif command.split(" ")[0] == "hid":
					hid = command.split(" ")[1]
				elif command.split(" ")[0] == "url":
					url = command.split(" ")[1]
				elif len(command) == 0 or command == "update ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not hid:
				print("too few arguments")
				return
			query = "hid%eq!" + hid + "!;datetime%eq!" + str(time.time()).split('.')[0] + "!;"
			if host:
				query += "host%eq!" + host + "!;"
			if password:
				query += "password%eq!" + password + "!;"
			if username:
				query += "uname%eq!" + username + "!;"
			if email:
				query += "email%eq!" + email + "!;"
			if notes:
				query += "notes%eq!" + notes + "!;"
			if url:
				query += "url%eq!" + url + "!;"
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("REQUPD" + query)
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
		
	def IOsync(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("REQSYNfetch_mode%eq!FETCH_SYNC!")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOall(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("REQSYNfetch_mode%eq!FETCH_ALL!")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOlogin(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\nlogin\n-u <username>\n-p <password>")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			username = None
			password = None
			for command in parameters:
				if command.split(" ")[0] == "u":
					username = command.split(" ")[1]
				elif command.split(" ")[0] == "p":
					password = command.split(" ")[1]
				elif len(command) == 0 or command == "login ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not username or not password:
				print("too few arguments")
				return
			finalPassword = CryptoHelper.SHA256(CryptoHelper.SHA256(password)[:32])
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGLGIusername%eq!" + username + "!;password%eq!" + finalPassword + "!;cookie%eq!" + ActiveConnection.cookie + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
	
	def IOconfirmNewDevice(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\nconfirmnewdevice\n-u <username>\n-p <password>\n-c <code> (numbers only)")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			username = None
			password = None
			code = None
			for command in parameters:
				if command.split(" ")[0] == "u":
					username = command.split(" ")[1]
				elif command.split(" ")[0] == "p":
					password = command.split(" ")[1]
				elif command.split(" ")[0] == "c":
					code = command.split(" ")[1]
				elif len(command) == 0 or command == "confirmnewdevice ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not username or not password or not code:
				print("too few arguments")
				return
			finalPassword = CryptoHelper.SHA256(CryptoHelper.SHA256(password)[:32])
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGCNDusername%eq!" + username + "!;code%eq!PM-" + code + "!;password%eq!" + finalPassword + "!;cookie%eq!" + ActiveConnection.cookie + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
		
	def IOverify(self, parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\nverify\n-u <username>\n-c <code> (numbers only)")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			username = None
			code = None
			for command in parameters:
				if command.split(" ")[0] == "u":
					username = command.split(" ")[1]
				elif command.split(" ")[0] == "c":
					code = command.split(" ")[1]
				elif len(command) == 0 or command == "verify ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not username or not code:
				print("too few arguments")
				return
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGVERusername%eq!" + username + "!;code%eq!PM-" + code + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
			
	def IOlogout(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLGO")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOsudo(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\nsu\n-p <password>")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			password = None
			for command in parameters:
				if command.split(" ")[0] == "p":
					password = command.split(" ")[1]
				elif len(command) == 0 or command == "su ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not password:
				print("too few arguments")
				return
			finalPassword = CryptoHelper.SHA256(CryptoHelper.SHA256(password)[:32])
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGADMpassword%eq!" + finalPassword + "!;cookie%eq!" + ActiveConnection.cookie + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
	
	def IOnewAdminDevice(self, parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\nnewadmindevice\n-p <password>\n-c <code> (numbers only)")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			password = None
			code = None
			for command in parameters:
				if command.split(" ")[0] == "p":
					password = command.split(" ")[1]
				elif command.split(" ")[0] == "c":
					code = command.split(" ")[1]
				elif len(command) == 0 or command == "newadmindevice ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not password or not code:
				print("too few arguments")
				return
			finalPassword = CryptoHelper.SHA256(CryptoHelper.SHA256(password)[:32])
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGNADpassword%eq!" + finalPassword + "!;code%eq!PM-" + code + "!;cookie%eq!" + ActiveConnection.cookie + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
			
	def IOinitAdminPwChange(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGAPR")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOinitPwChange(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGACRPWCmode%eq!PASSWORD_CHANGE!;")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOinitDelAccount(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGACRDELmode%eq!DELETE_ACCOUNT!;")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOcommitAdminPwChange(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\ncommitadminpwchange\n-p <password>\n-c <code> (numbers only)")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			password = None
			code = None
			for command in parameters:
				if command.split(" ")[0] == "p":
					password = command.split(" ")[1]
				elif command.split(" ")[0] == "c":
					code = command.split(" ")[1]
				elif len(command) == 0 or command == "commitadminpwchange ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not password or not code:
				print("too few arguments")
				return
			finalPassword = CryptoHelper.SHA256(CryptoHelper.SHA256(password)[:32]) 
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGAPCpassword%eq!" + finalPassword + "!;code%eq!PM-" + code + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
			
	def IOcommitPwChange(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\ncommitpwchange\n-p <password>\n-c <code> (numbers only)")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			password = None
			code = None
			for command in parameters:
				if command.split(" ")[0] == "p":
					password = command.split(" ")[1]
				elif command.split(" ")[0] == "c":
					code = command.split(" ")[1]
				elif len(command) == 0 or command == "commitpwchange ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not password or not code:
				print("too few arguments")
				return
			finalPassword = CryptoHelper.SHA256(CryptoHelper.SHA256(password)[:32])
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGCPCpassword%eq!" + finalPassword + "!;code%eq!PM-" + code + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
			
	def IOcommitDelAccount(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\ncommitdelaccount\n-c <code> (numbers only)")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			code = None
			for command in parameters:
				if command.split(" ")[0] == "c":
					code = command.split(" ")[1]
				elif len(command) == 0 or command == "commitdelaccount ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not code:
				print("too few arguments")
				return
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGDELcode%eq!PM-" + code + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
			
	def IObanClient(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\nbanclient\n-t <ip>\n-d <duration_in_seconds>")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			ip = None
			duration = None
			for command in parameters:
				if command.split(" ")[0] == "t":
					ip = command.split(" ")[1]
				elif command.split(" ")[0] == "d":
					duration = command.split(" ")[1]
				elif len(command) == 0 or command == "banclient ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not duration or not ip:
				print("too few arguments")
				return
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGBANip%eq!" + ip + "!;duration%eq!" + duration + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
			
	def IObanAccount(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\nbanaccount\n-u <username>\n-d <duration_in_secounds>")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			username = None
			duration = None
			for command in parameters:
				if command.split(" ")[0] == "u":
					username = command.split(" ")[1]
				elif command.split(" ")[0] == "d":
					duration = command.split(" ")[1]
				elif len(command) == 0 or command == "banaccount ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not username or not duration:
				print("too few arguments")
				return
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGBNAusername%eq!" + username + "!;duration%eq!" + duration + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
		
	def IOshutdown(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGSHTSHUTDOWN")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOreboot(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGRBTREBOOT")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOstart(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		ACThread = ActiveConnection()
		ACThread.start()
		
	def IOerror(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		print("SOCKET CLOSED (HANG UP SIMULATED)")
		ActiveConnection.clientSocket.close()
		
	def IOmasterPasswordRequest(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\nmasterpwrequest\n-u <username>")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			username = None
			for command in parameters:
				if command.split(" ")[0] == "u":
					username = command.split(" ")[1]
				elif len(command) == 0 or command == "masterpwrequest ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not username:
				print("too few arguments")
				return
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGPWHusername%eq!" + username + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
			
	def IOchangeEmailAddress(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\nchangeemailaddress\n-u <username>\n-e <new_email>")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			username = None
			email = None
			for command in parameters:
				if command.split(" ")[0] == "u":
					username = command.split(" ")[1]
				elif command.split(" ")[0] == "e":
					email = command.split(" ")[1]
				elif len(command) == 0 or command == "changeemailaddress ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not username or not email:
				print("too few arguments")
				return
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGCEAusername%eq!" + username + "!;cookie%eq!" + ActiveConnection.cookie + "!;new_email%eq!" + email + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
			
	def IOgetAccountActivity(self,parameters):
		try:
			if parameters[1] == "h":
				print("this command takes no parameters")
				return
		except Exception as e:
			print(e)
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGGAA")
		if ActiveConnection.debugging:
			print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOresendCode(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\nresendcode\n-u <username>\n-e <email>\n-n <name>")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			username = None
			email = None
			name = None
			for command in parameters:
				if command.split(" ")[0] == "u":
					username = command.split(" ")[1]
				elif command.split(" ")[0] == "e":
					email = command.split(" ")[1]
				elif command.split(" ")[0] == "n":
					name = command.split(" ")[1]
				elif len(command) == 0 or command == "resendcode ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not username or not email or not name:
				print("too few arguments")
				return
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGRTCusername%eq!" + username + "!;email%eq!" + email + "!;name%eq!" + name + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
			
	def IOchangeName(self,parameters):
		try:
			if parameters[1] == "h":
				print("arguments: \n\nchangename\n-n <new_name>")
				return
		except Exception as e:
			print(e)
			print("too few arguments")
		try:
			name = None
			for command in parameters:
				if command.split(" ")[0] == "n":
					name = command.split(" ")[1]
				elif len(command) == 0 or command == "changename ":
					pass
				else:
					print("invalid argument: -" + command)
					return
			if not name:
				print("too few arguments")
				return
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGCHNnew_name%eq!" + name + "!;")
			if ActiveConnection.debugging:
				print("SENDING: E" + AESCipher(ActiveConnection.aesKey).decrypt(cryptmsg))
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except Exception as e:
			print(e)
			print("typo in command \"" + " ".join(parameters) + "\"")
			
class ActiveConnection(Thread):
	clientSocket = None
	aesKey = None
	nonce = None
	foreignRsaKey = None
	cookie = None
	debugging = False
	def run(self):
		isDisconnected = False
		isSocketError = False
		isTcpFin = False
		server = "192.168.178.46"
		ActiveConnection.clientSocket = socket.create_connection((server, 4444))
		print('* * * CONNECTED * * *')
		IO.IOauth()
		try:
			buffer = b''
			while True:
				receiving = True
				#dataPackets = []
				while receiving:
					# LOAD DATA TO 32768 BYTE BUFFER
					data = self.clientSocket.recv(32768)
					if data:
						if ActiveConnection.debugging:
							print("DATA: " + str(data))
						dataPackets = None
						# CHECK IF PACKET IS VALID
						if b'\x04' in data and len(buffer) == 0:
							# GET DATA
							rawDataPackets = data.split(b'\x04')
							if ActiveConnection.debugging:
								print("rdp: " + str(rawDataPackets))
							lastDataPacket = rawDataPackets[len(rawDataPackets) - 1]
							if ActiveConnection.debugging:
								print("ldp: " + str(lastDataPacket))
							dataPackets = rawDataPackets[0:len(rawDataPackets) - 1]
							if ActiveConnection.debugging:
								print("dp: " + str(dataPackets))
							if not len(lastDataPacket) == 0:
								buffer += lastDataPacket
							receiving = False
						elif b'\x04' in data and not len(buffer) == 0:
							rawDataPackets = data.split(b'\x04')
							print("rdp: " + str(rawDataPackets))
							rawDataPackets[0] = buffer + rawDataPackets[0]
							if ActiveConnection.debugging:
								print("rdp final: " + str(rawDataPackets))
							buffer = b''
							lastDataPacket = rawDataPackets[len(rawDataPackets) - 1]
							if ActiveConnection.debugging:
								print("ldp: " + str(lastDataPacket))
							dataPackets = rawDataPackets[0:len(rawDataPackets) - 1]
							if ActiveConnection.debugging:
								print("dp: " + str(dataPackets))
							if not len(lastDataPacket) == 0:
								buffer += lastDataPacket
							receiving = False
						else:
							buffer += data
					else:
						isTcpFin = True
						return
				for dataPacket in dataPackets:
					if not dataPacket[0] == b'\x01':
						if dataPacket.count(b'\x01') == 0:
							print("CLIENT <-#- [ERRNO 01] IPT -#-> " + server)
							continue
						elif dataPacket.count(b'\x01') == 1:
							dataPacket = b'\x01' + dataPacket.split(b'\x01')[1]
						else:
							print("CLIENT <-#- [ERRNO 01] ISOH -#-> " + server)
							continue
					dataString = dataPacket[1:].decode("utf-8")
					if ActiveConnection.debugging:
						print("DS: " + dataString)
					# GET PACKET SPECIFIER
					packetSpecifier = dataString[:1]
					if ActiveConnection.debugging:
						print("PS: " + packetSpecifier)
					# CHECK IF DATA IS UNENCRYPTED
					if packetSpecifier == "U":
						# GET PACKETID
						packetID = dataString[1:4]
						# CHECK IF PACKET IS DEAUTH PACKET
						if ActiveConnection.debugging:
							print("PID: " + packetID)
						if packetID == "FIN":
							print("received FIN")
							isDisconnected = True
							# JUMP TO FINALLY AND FINISH CONNECTION
							try:
								print("REASON: " + dataString[4:].split("%eq")[1].replace("!",""))
							except:
								pass
							return
						elif packetID == "KEY":
							packetSID = dataString[4:7]
							if packetSID == "PEM":
								ActiveConnection.foreignRsaKey = dataString[7:].split("!")[1]
							# TODO: VALIDATE KEY
							ActiveConnection.nonce = hashlib.sha256(bytes(str(secrets.randbelow(10**50)), "utf-8")).hexdigest()
							encNonce = CryptoHelper.RSAEncrypt(ActiveConnection.nonce, ActiveConnection.foreignRsaKey)
							message = "key%eq!" + IO.pemKey + "!;nonce%eq!" + encNonce + "!;"
							ActiveConnection.clientSocket.send(b'\x01' + bytes("KCKE" + message, "utf-8") + b'\x04')
					# CHECK IF PACKET "IS KEY EXCHANGE" (RSA ENCRYPTED) 
					elif packetSpecifier == 'K':
						dec = CryptoHelper.RSADecrypt(dataString[1:], IO.privateKey)
						packetID = dec[:3]
						if packetID == "SKE":
							commandArray = dec[3:].split(";")
							key = None
							nonce = None
							for info in commandArray:
								if "key" in info:
									key = info.split("!")[1]
								elif "nonce" in info:
									nonce = info.split("!")[1]
							if not nonce == ActiveConnection.nonce:
								return
							ActiveConnection.aesKey = key
							ActiveConnection.nonce = hashlib.sha256(bytes(str(secrets.randbelow(10**50)), "utf-8")).hexdigest()
							message = "nonce%eq!" + ActiveConnection.nonce + "!;"
							aesEncryptor = AESCipher(ActiveConnection.aesKey)
							cryptmsg = aesEncryptor.encrypt("KEXACK" + message)
							ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
							if ActiveConnection.debugging:
								print("AES Key: " + ActiveConnection.aesKey)
					# CHECK IF PACKET IS AES ENCRYPTED
					elif packetSpecifier == 'E':
						# TODO: DECRYPT
						aesDecryptor = AESCipher(ActiveConnection.aesKey)
						decryptedData = aesDecryptor.decrypt(dataString[1:])
						print("SERVER: " + decryptedData)
						packetID = decryptedData[:3]
						packetSID = decryptedData[3:6]
						if packetID == "KEX":
							if packetSID == "ACK":
								if not decryptedData[6:].split("!")[1] == ActiveConnection.nonce:
									return
								print("Key exchange finished")
						elif packetID == "DTA":
							if packetSID == "RET":
								returnID = decryptedData[6:9]
								if returnID == "INS":
									valueArray = decryptedData[9:].split(';')
									localID = None
									hashedID = None
									for value in valueArray:
										if "local_id" in value:
											localID = value.split("%eq")[1].replace("!","")
										elif "hashed_id" in value:
											hashedID = value.split("%eq")[1].replace("!","")
									print("UPDATE local SET sid = \"" + hashedID + "\" WHERE id = " + localID + ";")
								elif returnID == "SYN":
									valueArray = decryptedData[9:].split('),(')
									for value in valueArray:
										valueTuple = value.replace(',',' | ').replace('(','').replace(')','')
										print(valueTuple)
							elif packetSID == "CKI":
								ActiveConnection.cookie = decryptedData[6:].split("!")[1]
								with open("cookie.txt", "w") as out:
									out.write(ActiveConnection.cookie)
									out.close()
						elif packetID == "LOG":
							if packetSID == "DMP":
								valueArray = decryptedData[12:].split('),(')
								for value in valueArray:
									valueTuple = value.replace(',',' | ').replace('(','').replace(')','')
									print(valueTuple)
					else:
						# INVALID PACKET SPECIFIER
						print("CLIENT <-#- [ERRNO 05] IPS -#-> " + server)
						return
		except SocketError as error:
			print("SOCKET ERROR: {0}".format(error))
			# SET SOCKET ERROR FLAG
			isSocketError = True
			pass
		# FREE THE SOCKET ONCE THE CLIENT DISCONNECTS OR THE CONNECTION FAILS
		finally:
			# CHECK IF SOCKET ERROR OCCURED
			if isSocketError:
				# RESET THE CONNECTION
				self.clientSocket.close()
				print("CLIENT <-x-  DISCONNECTED ERR  -x-> " + server)
			elif isTcpFin:
				# GOT TCP FIN
				try:
					self.clientSocket.shutdown(socket.SHUT_RDWR)
				except:
					pass
				# CLOSE SOCKET
				self.clientSocket.close()
				print ("CLIENT <-x-  DISCONNECTED FIN  -x-> " + server)
			else:
				if not isDisconnected:
					# SEND CUSTOM FIN
					print("SENDING FIN")
					self.clientSocket.send(b'\x01' + bytes("UFIN", "utf-8") + b'\x04')
				# SEND TCP FIN
				self.clientSocket.shutdown(socket.SHUT_RDWR)
				# CLOSE SOCKET
				self.clientSocket.close()
				print ("CLIENT <-x-  DISCONNECTED  -x-> " + server)

os.system('cls' if os.name == 'nt' else 'clear')
print('DEBUG CLIENT 0.11-8.18\n')
IOThread = IO()
IOThread.start()
try:
	with open("cookie.txt", "r") as fileIn:
		ActiveConnection.cookie = fileIn.readline()
		fileIn.close
except:
	print("NO COOKIE FOUND")
