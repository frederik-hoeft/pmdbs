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
				command = input("")
				if command == 'auth':
					self.IOauth()
				elif command == "exit":
					self.IOexit()
				elif command == 'register':
					self.IOregister()
				elif command == 'insert':
					self.IOinsert()
				elif command == 'select':
					self.IOselect()
				elif command == 'update':
					self.IOupdate()
				elif command == 'msg':
					self.IOmsg()
				elif command == 'sync':
					self.IOsync()
				elif command == 'all':
					self.IOall()
				elif command == 'login':
					self.IOlogin()
				elif command == "logout":
					self.IOlogout()
				elif command == 'sudo':
					self.IOsudo()
				elif command == 'shutdown':
					self.IOshutdown()
				elif command == 'reboot':
					self.IOreboot()
				elif command == 'start':
					self.IOstart()
				elif command == "slog":
					self.IOslog()
				elif command == "clog":
					self.IOclog()
				elif command == "listallc":
					self.IOlistAllClients()
				elif command == "error":
					self.IOerror()
				elif command[:4] == "kick":
					self.IOkick(command)
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
	
	def IOlistAllClients(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLICmode%eq!ALL_CONNECTED!")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
	
	def IOslog(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLOGSERVER")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
	
	def IOclog(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLOGCLIENT")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04') 
	   
	def IOauth():
		ActiveConnection.clientSocket.send(b'\x01' + bytes("UPPK" + IO.pemKey, "utf-8") + b'\x04')
		
	def IOexit(self):
		ActiveConnection.clientSocket.send(b'\x01' + bytes("UFIN", "utf-8") + b'\x04')
		ActiveConnection.clientSocket.shutdown(socket.SHUT_RDWR)
		ActiveConnection.clientSocket.close()
		exit()
	
	def IOmsg(self):
		ActiveConnection.clientSocket.send(b'\x01' + bytes("UHello", "utf-8") + b'\x04')
		
	def IOregister(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGREGUSER\x1f5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8\x1ffrederik.hoeft@gmail.com")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOinsert(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("REQINShost%eq!test!;password%eq!test!;datetime%eq!20180831183209!;\x1f12")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOselect(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("REQSEL7mRYoHaxRgpN+KH8mb92xKc1F2m4BWKMXS5u0vRCqND/5WZ5JWBcue/LVrrcwOJCfzbB5vu1bhaMQPSwlUmgpw==;VjjBgvgnhGPLKijI4BjRoJgILrX6SlHeIjBmWjhQXZT6nk52yUWkcLeQjFgoEsJmYIQmWacuJ61rIHtHQm8fQQ==;mKolv7XwxCxCJr6oAHTNJjtuKmFY4lLIEecE1CXGiQUBlFGJ4gBrAHx3JfH9X4yXzOP7iSYMg5QT7We1Bd1LUQ==;JKqL9rE0NciMWP+VOdVycGA+O29+07TrTiKlyhoq7D/T37eQ1n+idoUOsk0wDPoIl4e8kif/0dPj3lRSzpXEpA==;VgYstSzzgYg/12aoVFQp5ijP2cHRND8W6P0kQioYRFleth75OSKDa2snh7R+mMmq1TjURQWM0B98QeCcwG6T0A==;E1aP9yowzxdpTb7bmU3pmfvEai8i81oeD+Nn27XL8U9ujkVwf7T0gJI/kPie1BbX4N5TNPbPel4srKRYqLSlPA==;")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOupdate(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("REQUPDhost%eq!test!;password%eq!password!;datetime%eq!20180831183209!;hid%eq!VjjBgvgnhGPLKijI4BjRoJgILrX6SlHeIjBmWjhQXZT6nk52yUWkcLeQjFgoEsJmYIQmWacuJ61rIHtHQm8fQQ==!;\x1f12")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOsync(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("REQSYNfetch_mode%eq!FETCH_SYNC!")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOall(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("REQSYNfetch_mode%eq!FETCH_ALL!")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOlogin(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLGIUSER\x1f5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOlogout(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGLGO")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOsudo(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGADM__ADMIN__\x1f5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOshutdown(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGSHTSHUTDOWN")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOreboot(self):
		aesEncryptor = AESCipher(ActiveConnection.aesKey)
		cryptmsg = aesEncryptor.encrypt("MNGRBTREBOOT")
		ActiveConnection.clientSocket.send(b'\x01' + bytes("E" + cryptmsg, "utf-8") + b'\x04')
		
	def IOstart(self):
		ACThread = ActiveConnection()
		ACThread.start()
		
	def IOerror(self):
		print("SOCKET CLOSED (HANG UP SIMULATED)")
		ActiveConnection.clientSocket.close()
			
class ActiveConnection(Thread):
	clientSocket = None
	aesKey = None
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
					# CHECK IF PACKET "IS KEY EXCHANGE" (RSA ENCRYPTED) 
					elif packetSpecifier == 'K':
						key = dataString[4:]
						ActiveConnection.aesKey = CryptoHelper.RSADecrypt(key, IO.privateKey)
						print("AES: " + ActiveConnection.aesKey)
					# CHECK IF PACKET IS AES ENCRYPTED
					elif packetSpecifier == 'E':
						# TODO: DECRYPT
						aesDecryptor = AESCipher(ActiveConnection.aesKey)
						decryptedData = aesDecryptor.decrypt(dataString[1:])
						print("SERVER (AES)" + decryptedData)
						packetID = decryptedData[:3]
						packetSID = decryptedData[3:6]
						if packetID == "DTA":
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

