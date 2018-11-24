# CONFIG
# VERSION INFO: 0.11-4.18

# CHANGE SERVER SETTINGS HERE
# ALL OF THESE VALUES *MUST* BE SET --> LIMITED ERROR HANDLING
# IN ORDER FOR CHANGES TO TAKE EFFECT, THE SERVER MUST BE RESTARTED 


REBOOT_TIME = 1                     # TIME IN SECONDS TO WAIT INBETWEEN SHUTDOWN OR REBOOT STEPS (DEPENDING ON SERVER LAG AND USER COUNT YOU MIGHT WANT TO INCREMENT THIS VALUE)
LOCAL_ADDRESS = ""                  # LOCAL IPv4 ADDRESS OF HOSTING MACHINE (I.E. 192.168.X.X or 10.X.X.X)
LOCAL_PORT = 0                      # LOCAL PORT FOR THE HOSTING MACHINE TO LISTEN ON

# SQL SETTINGS                      # LEAVE AS IS
TABLE_USER_LENGTH = 15
TABLE_CLIENTLOG_LENGTH = 6
TABLE_BLACKLIST_LENGTH = 4
TABLE_CONNECTUSERCOOKIES_LENGTH = 2
TABLE_COOKIES_LENGTH = 2
TABLE_DATA_LENGTH = 10
TABLE_SERVERLOG_LENGTH = 4

# 2FA SETTINGS                      # TIME IN SECONDS UNTIL 2FA CODES EXPIRE
ACCOUNT_ACTIVATION_MAX_TIME = 3600
DELETE_ACCOUNT_CONFIRMATION_MAX_TIME = 1800
MAX_CODE_ATTEMPTS = 3
MAX_CODE_ATTEMPTS_ADMIN = 3
NEW_DEVICE_CONFIRMATION_MAX_TIME = 1800
NEW_DEVICE_CONFIRMATION_ADMIN_MAX_TIME = 1800
RESEND_CODE_MAX_COUNT = 3
PASSWORD_CHANGE_CONFIRMATION_MAX_TIME = 1800
PASSWORD_CHANGE_CONFIRMATION_ADMIN_MAX_TIME = 1800
WRONG_CODE_AUTOBAN_DURATION = 3600
WRONG_CODE_AUTOBAN_DURATION_ADMIN = 86400

# EMAIL SETTINGS
SUPPORT_EMAIL_HOST = ""             # DOMAIN NAME OF SMTP SERVICE (I.E. smtp.gmail.com)
SUPPORT_EMAIL_SSL_PORT = 0          # SSL ENABLED PORT OF SMTP SERVICE
SUPPORT_EMAIL_ADDRESS = ""          # YOUR EMAIL ADDRESS
SUPPORT_EMAIL_PASSWORD = ""         # YOUR EMAIL PASSWORD
SUPPORT_EMAIL_LOGO = "icon.png"
SUPPORT_EMAIL_SENDER = "PMDBS Support"

##################################################################################
#								EMAIL PRESETS
##################################################################################
# DELETE ACCOUNT
SUPPORT_EMAIL_DELETE_ACCOUNT_SUBJECT = "[PMDBS] Delete your account?"
SUPPORT_EMAIL_DELETE_ACCOUNT_PLAIN_TEXT = "Dear %s,\n\nYou have requested to delete your account and all data associated to it.\nThe request originated from the following device:\n\n%s\n\nALL DATA WILL BE PERMANENTLY DELETED AND CANNOT BE RECOVERED!\nTo confirm your request, please enter the code below when prompted:\n\n%s\n\nTime left until the code expires: %s.\n\nBest regards,\nPMDBS Support Team"
SUPPORT_EMAIL_DELETE_ACCOUNT_HTML_TEXT = "<html><head><style>table.main {width:800px;background-color:#212121;color:#FFFFFF;margin:auto;border-collapse:collapse;}td.top {padding: 50px 50px 0px 50px;}td.header {background-color:#212121;color:#FF6031;padding: 0px 50px 0px 50px;}td.text {padding: 0px 50px 0px 50px;}td.bottom {padding: 0px 50px 50px 50px;}</style></head><body><table class=\"main\"><tr><td class=\"top\" align=\"center\"><img src=\"cid:icon1\" width=\"100\" height=\"100\"></td></tr><tr><td class=\"header\"><h3>Dear %s,</h3></td></tr><tr><td class=\"text\"><p>You have requested to delete your account and all data associated to it.<br>The request originated from the following device:<br><br>%s<br><br><b>ALL DATA WILL BE PERMANENTLY DELETED AND CANNOT BE RECOVERED!</b><br><br><br>To confirm your request, please enter the code below when prompted:</p></td></tr><tr><td class=\"header\"><p align=\"center\"><b>%s</b></p></td></tr><tr><td class=\"bottom\"><p><br>Time left until the code expires: %s.<br><br>Best regards,<br>PMDBS Support Team</p></td></tr></table></body></html>"
# LOGIN FROM NEW DEVICE
SUPPORT_EMAIL_NEW_DEVICE_SUBJECT = "[PMDBS] Security warning"
SUPPORT_EMAIL_NEW_DEVICE_PLAIN_TEXT = "Dear %s,\n\nAre you trying to log in from a new device?\n\nSomeone just tried to log into your account using the following device:\n\n%s\n\nYou have received this email because we want to make sure that this is really you.\nTo verify that it is you, please enter the following code when prompted:\n\n%s\n\nTime left until the code expires: %s.\n\nIf you did not try to sign in, you should consider changing your master password. There is no need to panic though, your account is save as long as your email is not compromized as well.\n\nBest regards,\nPMDBS Support Team"
SUPPORT_EMAIL_NEW_DEVICE_HTML_TEXT = "<html><head><style>table.main {width:800px;background-color:#212121;color:#FFFFFF;margin:auto;border-collapse:collapse;}td.top {padding: 50px 50px 0px 50px;}td.header {background-color:#212121;color:#FF6031;padding: 0px 50px 0px 50px;}td.text {padding: 0px 50px 0px 50px;color:#FFFFFF;}td.bottom {padding: 0px 50px 50px 50px;}</style></head><body><table class=\"main\"><tr><td class=\"top\" align=\"center\"><img src=\"cid:icon1\" width=\"100\" height=\"100\"></td></tr><tr><td class=\"header\"><h2>Dear %s,</h2></td></tr><tr><td class=\"text\"><p><b>Are you trying to log in from a new device?</b><br><br>Someone just tried to log into your account using the following device:<br><br>%s<br><br>You have received this email because we want to make sure that this is really you.<br>To verify that it is you, please enter the following code when prompted:</p></td></tr><tr><td class=\"header\"><p align=\"center\"><b>%s</b></p></td></tr><tr><td class=\"bottom\"><p><br><br>Time left until the code expires: %s.<br><br>If you did not try to sign in, you should consider <b>changing your master password</b>. There is no need to panic though, your account is save as long as your email is not compromized as well.<br><br>Best regards,<br>PMDBS Support Team</p></td></tr></table></body></html>"
# PASSWORD CHANGE
SUPPORT_EMAIL_PASSWORD_CHANGE_SUBJECT = "[PMDBS] Password change"
SUPPORT_EMAIL_PASSWORD_CHANGE_PLAIN_TEXT = "Dear %s\n\nYou have requested to change your master password in our app.\nThe request originated from the following device:\n\n%s\n\nTo change your password, please enter the code below when prompted:\n\n%s\n\nTime left until the code expires: %s.\n\nBest regards,\nPMDBS Support Team"
SUPPORT_EMAIL_PASSWORD_CHANGE_HTML_TEXT = "<html><head><style>table.main {width:800px;background-color:#212121;color:#FFFFFF;margin:auto;border-collapse:collapse;}td.top {padding: 50px 50px 0px 50px;}td.header {background-color:#212121;color:#FF6031;padding: 0px 50px 0px 50px;}td.text {padding: 0px 50px 0px 50px;}td.bottom {padding: 0px 50px 50px 50px;}</style></head><body><table class=\"main\"><tr><td class=\"top\" align=\"center\"><img src=\"cid:icon1\" width=\"100\" height=\"100\"></td></tr><tr><td class=\"header\"><h3>Dear %s,</h3></td></tr><tr><td class=\"text\"><p>You have requested to change your master password in our app. The request originated from the following device:<br><br>%s<br><br>To change your password, please enter the code below when prompted:</p></td></tr><tr><td class=\"header\"><p align=\"center\"><b>%s</b></p></td></tr><tr><td class=\"bottom\"><p><br>Time left until the code expires: %s.<br><br>Best regards,<br>PMDBS Support Team</p></td></tr></table></body></html>"
# REGISTER
SUPPORT_EMAIL_REGISTER_SUBJECT = "[PMDBS] Please verify your email address."
SUPPORT_EMAIL_REGISTER_PLAIN_TEXT = "Welcome, %s!\n\nThe Password Management Database System enables you to securely store your passwords and confident information in one place and allows an easy access from all your devices.\n\nTo verify your account, please enter the following code when prompted:\n\n%s\n\nTime left until the code expires: %s.\n\nBest regards,\nPMDBS Support Team"
SUPPORT_EMAIL_REGISTER_HTML_TEXT = "<html><head><style>table.main {width:800px;background-color:#212121;color:#FFFFFF;margin:auto;border-collapse:collapse;}td.top {padding: 50px 50px 0px 50px;}td.header {background-color:#212121;color:#FF6031;padding: 0px 50px 0px 50px;}td.text {padding: 0px 50px 0px 50px;color:#FFFFFF;}td.bottom {padding: 0px 50px 50px 50px;}</style></head><body><table class=\"main\"><tr><td class=\"top\" align=\"center\"><img src=\"cid:icon1\" width=\"100\" height=\"100\"></td></tr><tr><td class=\"header\"><h2>Welcome, %s!</h2></td></tr><tr><td class=\"text\"><p>The Password Management Database System enables you to securely store your passwords and confident information in one place and allows an easy access from all your devices.<br><br>To verify your account, please enter the following code when prompted:</p></td></tr><tr><td class=\"header\"><p align=\"center\"><b>%s</b></p></td></tr><tr><td class=\"bottom\"><p><br><br>Time left until the code expires: %s.<br><br>Best regards,<br>PMDBS Support Team</p></td></tr></table></body></html>"
# ADMIN LOGIN FROM NEW DEVICE
SUPPORT_EMAIL_NEW_ADMIN_DEVICE_SUBJECT = "[PMDBS] Admin security warning"
SUPPORT_EMAIL_NEW_ADMIN_DEVICE_PLAIN_TEXT = "Hey Admin!\n\nAre you trying to log in from a new device?\n\nSomeone just tried to log in as admin using the following device:\n\n%s\n\nYou have received this email because we want to make sure that this is really you.\nTo verify that it is you, please enter the following code when prompted:\n\n%s\n\nTime left until the code expires: %s.\n\nIf you did not try to sign in, you should consider changing the admin password.\n\nBest regards,\nPMDBS Support Team"
SUPPORT_EMAIL_NEW_ADMIN_DEVICE_HTML_TEXT = "<html><head><style>table.main {width:800px;background-color:#212121;color:#FFFFFF;margin:auto;border-collapse:collapse;}td.top {padding: 50px 50px 0px 50px;}td.header {background-color:#212121;color:#FF6031;padding: 0px 50px 0px 50px;}td.text {padding: 0px 50px 0px 50px;color:#FFFFFF;}td.bottom {padding: 0px 50px 50px 50px;}</style></head><body><table class=\"main\"><tr><td class=\"top\" align=\"center\"><img src=\"cid:icon1\" width=\"100\" height=\"100\"></td></tr><tr><td class=\"header\"><h2>Hey Admin!</h2></td></tr><tr><td class=\"text\"><p><b>Are you trying to log in from a new device?</b><br><br>Someone just tried to log in as admin using the following device:<br><br>%s<br><br>You have received this email because we want to make sure that this is really you.<br>To verify that it is you, please enter the following code when prompted:</p></td></tr><tr><td class=\"header\"><p align=\"center\"><b>%s</b></p></td></tr><tr><td class=\"bottom\"><p><br><br>Time left until the code expires: %s.<br><br>If you did not try to sign in, you should consider <b>changing the admin password!</b><br><br>Best regards,<br>PMDBS Support Team</p></td></tr></table></body></html>"
# ADMIN PASSWORD CHANGE
SUPPORT_EMAIL_PASSWORD_CHANGE_ADMIN_SUBJECT = "[PMDBS] Admin password change"
SUPPORT_EMAIL_PASSWORD_CHANGE_ADMIN_PLAIN_TEXT = "Hey Admin!\n\nYou have requested to change the admin password.\nThe request originated from the following device:\n\n%s\n\nTo change your password, please enter the code below when prompted:\n\n%s\n\nTime left until the code expires: %s.\nIf you did not request this email then there's someone out there playing around with admin privileges.\n*You should probably do something about that*\n\nBest regards,\nPMDBS Support Team"
SUPPORT_EMAIL_PASSWORD_CHANGE_ADMIN_HTML_TEXT = "<html><head><style>table.main {width:800px;background-color:#212121;color:#FFFFFF;margin:auto;border-collapse:collapse;}td.top {padding: 50px 50px 0px 50px;}td.header {background-color:#212121;color:#FF6031;padding: 0px 50px 0px 50px;}td.text {padding: 0px 50px 0px 50px;}td.bottom {padding: 0px 50px 50px 50px;}</style></head><body><table class=\"main\"><tr><td class=\"top\" align=\"center\"><img src=\"cid:icon1\" width=\"100\" height=\"100\"></td></tr><tr><td class=\"header\"><h3>Hey Admin!</h3></td></tr><tr><td class=\"text\"><p>You have requested to change the admin password.<br>The request originated from the following device:<br><br>%s<br><br>To change your password, please enter the code below when prompted:</p></td></tr><tr><td class=\"header\"><p align=\"center\"><b>%s</b></p></td></tr><tr><td class=\"bottom\"><p><br>Time left until the code expires: %s.<br>If you did not request this email then there's someone out there playing around with admin privileges.<br><b>*You should probably do something about that*</b><br><br>Best regards,<br>PMDBS Support Team</p></td></tr></table></body></html>"
# VERSION INFO
CONFIG_VERSION = "0.11-8.18"
CONFIG_BUILD = "development"
