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
import hashlib
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

	def __init__(self, key): 
		self.bs = 32
		self.key = hashlib.sha256(key.encode()).digest()

	def encrypt(self, raw):
		raw = raw.encode("utf-8")
		raw = self._pad(raw)
		iv = Random.new().read(AES.block_size)
		cipher = AES.new(self.key, AES.MODE_CBC, iv)
		return base64.b64encode(iv + cipher.encrypt(raw)).decode("utf-8")

	def decrypt(self, enc):
		enc = base64.b64decode(enc)
		iv = enc[:AES.block_size]
		cipher = AES.new(self.key, AES.MODE_CBC, iv)
		return self._unpad(cipher.decrypt(enc[AES.block_size:])).decode("utf-8")

	def _pad(self, s):
		return s + (self.bs - len(s) % self.bs) * chr(self.bs - len(s) % self.bs).encode("utf-8")

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
				parameters = input("").split(" ")
				command = parameters[0]
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
				elif command == "slog":
					self.IOslog(parameters)
				elif command == "clog":
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
				else:
					print('unknown command ' + '\'' + command + '\'')
		except:
			pass
			
	def IOkick(self, command):
		try:
			commands = command.split(" ")
			mode = commands[1]
			target = commands[2]
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGKIKmode%eq!" + mode + "!;target%eq!" + target + "!")
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except:
			print('typo in command ' + '\'' + command + '\'')
	
	def IOpasswordChangeRequest(self, parameters):
		try:
			if parameters[1] == "--help":
				print("this command takes no parameters")
				return
		except:
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGIPCmode%eq!PASSWORD_CHANGE!;")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
	
	def IOlistAllClients(self,parameters):
		try:
			if parameters[1] == "--help":
				print("this command takes no parameters")
				return
		except:
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLICmode%eq!ALL_CONNECTED!")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
	
	def IOslog(self,parameters):
		try:
			if parameters[1] == "--help":
				print("this command takes no parameters")
				return
		except:
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLOGSERVER")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
	
	def IOclog(self,parameters):
		try:
			if parameters[1] == "--help":
				print("this command takes no parameters")
				return
		except:
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLOGCLIENT")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04') 
	   
	def IOauth():
		ActiveConnection.clientSocket.send(b'\x01' + bytes("UINIPEM", "utf-8") + b'\x04')
		
	def IOexit(self,parameters):
		try:
			if parameters[1] == "--help":
				print("this command takes no parameters")
				return
		except:
			pass
		ActiveConnection.clientSocket.send(b'\x01' + bytes("UFIN", "utf-8") + b'\x04')
		exit()
		
	def IOcookie(self,parameters):
		try:
			if parameters[1] == "--help":
				print("this command takes no parameters")
				return
		except:
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGCKI")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
	
	def IOmsg(self,parameters):
		try:
			if parameters[1] == "--help":
				print("this command takes no parameters")
				return
		except:
			pass
		ActiveConnection.clientSocket.send(b'\x01' + bytes("UHello", "utf-8") + b'\x04')
		
	def IOregister(self, parameters):
		try:
			if parameters[1] == "--help":
				print("\"register <username> <password> <email> <nickname>\"")
				return
		except:
			print("too few arguments")
		try:
			username = parameters[1]
			password = parameters[2]
			email = parameters[3]
			nickname = parameters[4]
			finalPassword = CryptoHelper.SHA256(CryptoHelper.SHA256(password)[:32])
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGREGusername%eq!" + username + "!;password%eq!" + finalPassword + "!;email%eq!" + email + "!;nickname%eq!" + nickname + "!;cookie%eq!" + ActiveConnection.cookie + "!;")
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except:
			print("typo in command \"" + " ".join(parameters) + "\"")
		
	def IOinsert(self,parameters):
		try:
			if parameters[1] == "--help":
				print("insert <localID> <username> <password> <host> <notes> <email> <datetime>")
				return
		except:
			pass
		try:
			localID = parameters[1]
			uname = parameters[2]
			password = parameters[3]
			host = parameters[4]
			notes = parameters[5]
			email = parameters[6]
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("REQINSuname&eq!" +  + "!;password%eq!" +  + "!;host&eq!" +  + "!;notes%eq!" +  + "!;email%eq!" +  + "!;datetime%eq!" + str(time.time()).split('.')[0] + "!;\x1f" + localID)
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except:
			print("typo in command \"" + " ".join(parameters) + "\"")
		
	def IOselect(self,parameters):
		try:
			if parameters[1] == "--help":
				print("select HID1 HID2 HID3 [...]")
				return
		except:
			print("too few arguments")
		hidString = ""
		for HID in parameters[1:]:
			hidString += HID + ";"
		if hidString == "":
			print("too few arguments")
			return
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("REQSEL" + hidString)
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOupdate(self,parameters):
		try:
			if parameters[1] == "--help":
				print("TODO...")
				return
		except:
			print("too few arguments")
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("REQUPDhost%eq!test!;password%eq!password!;datetime%eq!20180831183209!;hid%eq!VjjBgvgnhGPLKijI4BjRoJgILrX6SlHeIjBmWjhQXZT6nk52yUWkcLeQjFgoEsJmYIQmWacuJ61rIHtHQm8fQQ==!;\x1f12")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOsync(self,parameters):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("REQSYNfetch_mode%eq!FETCH_SYNC!")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOall(self,parameters):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("REQSYNfetch_mode%eq!FETCH_ALL!")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOlogin(self,parameters):
		try:
			if parameters[1] == "--help":
				print("login <username> <password>")
				return
		except:
			print("too few arguments")
		try:
			username = parameters[1]
			password = CryptoHelper.SHA256(CryptoHelper.SHA256(parameters[2])[:32])
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGLGIusername%eq!" + username + "!;password%eq!" + password + "!;cookie%eq!" + ActiveConnection.cookie + "!;")
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except:
			print("typo in command \"" + " ".join(parameters) + "\"")
	
	def IOconfirmNewDevice(self,parameters):
		try:
			if parameters[1] == "--help":
				print("confirmnewdevice <username> <password> <code>")
				return
		except:
			print("too few arguments")
		try:
			username = parameters[1]
			password = CryptoHelper.SHA256(CryptoHelper.SHA256(parameters[2])[:32])
			code = parameters[3]
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGCNDusername%eq!" + username + "!;code%eq!" + code + "!;password%eq!" + password + "!;cookie%eq!" + ActiveConnection.cookie + "!;")
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except:
			print("typo in command \"" + " ".join(parameters) + "\"")
		
	def IOverify(self, parameters):
		try:
			if parameters[1] == "--help":
				print("verify <username> <code>")
				return
		except:
			print("too few arguments")
		try:
			username = parameters[1]
			code = parameters[2]
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGVERusername%eq!" + username + "!;code%eq!" + code + "!;")
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except:
			print("typo in command \"" + " ".join(parameters) + "\"")
			
	def IOlogout(self,parameters):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLGO")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOsudo(self,parameters):
		try:
			if parameters[1] == "--help":
				print("su <password>")
				return
		except:
			print("too few arguments")
		try:
			password = CryptoHelper.SHA256(CryptoHelper.SHA256(parameters[1])[:32])
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGADMpassword%eq!" + password + "!;cookie%eq!" + ActiveConnection.cookie + "!;")
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except:
			print("typo in command \"" + " ".join(parameters) + "\"")
	
	def IOnewAdminDevice(self, parameters):
		try:
			if parameters[1] == "--help":
				print("newadmindevice <password> <code>")
				return
		except:
			print("too few arguments")
		try:
			password = CryptoHelper.SHA256(CryptoHelper.SHA256(parameters[1])[:32])
			code = parameters[2]
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGNADpassword%eq!" + password + "!;code%eq!" + code + "!;cookie%eq!" + ActiveConnection.cookie + "!;")
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except:
			print("typo in command \"" + " ".join(parameters) + "\"")
			
	def IOinitAdminPwChange(self,parameters):
		try:
			if parameters[1] == "--help":
				print("this command takes no parameters")
				return
		except:
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGAPR")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOinitPwChange(self,parameters):
		try:
			if parameters[1] == "--help":
				print("this command takes no parameters")
				return
		except:
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGACRPWC")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOinitDelAccount(self,parameters):
		try:
			if parameters[1] == "--help":
				print("this command takes no parameters")
				return
		except:
			pass
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGACRDEL")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOcommitAdminPwChange(self,parameters):
		try:
			if parameters[1] == "--help":
				print("commitadminpwchange <new_password> <code>")
				return
		except:
			print("too few arguments")
		try:
			password = CryptoHelper.SHA256(CryptoHelper.SHA256(parameters[1])[:32])
			code = parameters[2] 
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGAPCpassword%eq!" + password + "!;code%!" + code + "!;")
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except:
			print("typo in command \"" + " ".join(parameters) + "\"")
			
	def IOcommitPwChange(self,parameters):
		try:
			if parameters[1] == "--help":
				print("commitpwchange <new_password> <code>")
				return
		except:
			print("too few arguments")
		try:
			password = CryptoHelper.SHA256(CryptoHelper.SHA256(parameters[1])[:32])
			code = parameters[2] 
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGCPCpassword%eq!" + password + "!;code%!" + code + "!;")
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except:
			print("typo in command \"" + " ".join(parameters) + "\"")
			
	def IOcommitDelAccount(self,parameters):
		try:
			if parameters[1] == "--help":
				print("commitdelaccount <code>")
				return
		except:
			print("too few arguments")
		try:
			password = CryptoHelper.SHA256(CryptoHelper.SHA256(parameters[1])[:32])
			code = parameters[2] 
			aesEncryptor = AESCipher(ActiveConnection.aesKey)
			cryptmsg = aesEncryptor.encrypt("MNGDELcode%!" + code + "!;")
			ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		except:
			print("typo in command \"" + " ".join(parameters) + "\"")
		
	def IOshutdown(self,parameters):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGSHTSHUTDOWN")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOreboot(self,parameters):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGRBTREBOOT")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOstart(self,parameters):
		ACThread = ActiveConnection()
		ACThread.start()
		
	def IOerror(self,parameters):
		print("SOCKET CLOSED (HANG UP SIMULATED)")
		ActiveConnection.clientSocket.close()
			
class ActiveConnection(Thread):
	clientSocket = None
	aesKey = None
	nonce = None
	foreignRsaKey = None
	cookie = None
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
						print("DATA: " + str(data))
						dataPackets = None
						# CHECK IF PACKET IS VALID
						if b'\x04' in data and len(buffer) == 0:
							# GET DATA
							rawDataPackets = data.split(b'\x04')
							print("rdp: " + str(rawDataPackets))
							lastDataPacket = rawDataPackets[len(rawDataPackets) - 1]
							print("ldp: " + str(lastDataPacket))
							dataPackets = rawDataPackets[0:len(rawDataPackets) - 1]
							print("dp: " + str(dataPackets))
							if not len(lastDataPacket) == 0:
								buffer += lastDataPacket
							receiving = False
						elif b'\x04' in data and not len(buffer) == 0:
							rawDataPackets = data.split(b'\x04')
							print("rdp: " + str(rawDataPackets))
							rawDataPackets[0] = buffer + rawDataPackets[0]
							print("rdp final: " + str(rawDataPackets))
							buffer = b''
							lastDataPacket = rawDataPackets[len(rawDataPackets) - 1]
							print("ldp: " + str(lastDataPacket))
							dataPackets = rawDataPackets[0:len(rawDataPackets) - 1]
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
					print("DS: " + dataString)
					# GET PACKET SPECIFIER
					packetSpecifier = dataString[:1]
					print("PS: " + packetSpecifier)
					# CHECK IF DATA IS UNENCRYPTED
					if packetSpecifier == "U":
						# GET PACKETID
						packetID = dataString[1:4]
						# CHECK IF PACKET IS DEAUTH PACKET
						print("PID: " + packetID)
						if packetID == "FIN":
							print("received FIN")
							isDisconnected = True
							# JUMP TO FINALLY AND FINISH CONNECTION
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
							print("AES: " + ActiveConnection.aesKey)
					# CHECK IF PACKET IS AES ENCRYPTED
					elif packetSpecifier == 'E':
						# TODO: DECRYPT
						aesDecryptor = AESCipher(ActiveConnection.aesKey)
						decryptedData = aesDecryptor.decrypt(dataString[1:])
						print("SERVER (AES)" + decryptedData)
						packetID = decryptedData[:3]
						packetSID = decryptedData[3:6]
						if packetID == "KEX":
							if packetSID == "ACK":
								if not decryptedData[6:].split("!")[1] == ActiveConnection.nonce:
									return
								print("CORRECT :)")
						elif packetID == "DTA":
							if packetSID == "RET":
								returnID = decryptedData[6:9]
								if returnID == "INS":
									valueArray = decryptedData[9:].split('\x1f')
									localID = valueArray[0]
									hashedID = valueArray[1]
									print("UPDATE local SET sid = \"" + hashedID + "\" WHERE id = " + localID + ";")
								elif returnID == "SYN":
									valueArray = decryptedData[9:].split('),(')
									for value in valueArray:
										valueTuple = value.replace(',',' | ').replace('(','').replace(')','')
										print(valueTuple)
							elif packetSID == "CKI":
								ActiveConnection.cookie = decryptedData[6:].split("!")[1]
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
				# SEND TCP FIN
				self.clientSocket.shutdown(socket.SHUT_RDWR)
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
print('DEBUG CLIENT v0.45\n')
IOThread = IO()
IOThread.start()

