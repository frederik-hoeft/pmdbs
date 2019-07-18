import ssl
import smtplib
# CONNECT TO SMTP SERVER (SSL)
context = ssl.create_default_context()
server = smtplib.SMTP(host = "smtp-mail.outlook.com", port = 587)
server.ehlo()
server.starttls(context = context)
server.ehlo()
# LOGIN
try:
	server.login("mail@outlook.com", "password")
except Exception as e:
	Handle.Error("MAIL", e, clientAddress, None, None, False)
else:
	print("success!")
finally:
	server.quit()
