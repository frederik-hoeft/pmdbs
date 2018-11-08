# CONFIG
# VERSION INFO: 0.11-2.18

# CHANGE SERVER SETTINGS HERE
# ALL OF THESE VALUES *MUST* BE SET --> LIMITED ERROR HANDLING
# IN ORDER FOR CHANGES TO TAKE EFFECT, THE SERVER MUST BE RESTARTED 


REBOOT_TIME = 1                     # TIME IN SECONDS TO WAIT INBETWEEN SHUTDOWN OR REBOOT STEPS (DEPENDING ON SERVER LAG AND USER COUNT YOU MIGHT WANT TO INCREMENT THIS VALUE)
LOCAL_ADDRESS = ""                  # LOCAL IPv4 ADDRESS OF HOSTING MACHINE (I.E. 192.168.X.X or 10.X.X.X)
LOCAL_PORT = 0                      # LOCAL PORT FOR THE HOSTING MACHINE TO LISTEN ON
SUPPORT_EMAIL_HOST = ""             # DOMAIN NAME OF SMTP SERVICE (I.E. smtp.gmail.com)
SUPPORT_EMAIL_SSL_PORT = 0          # SSL ENABLED PORT OF SMTP SERVICE
SUPPORT_EMAIL_ADDRESS = ""          # YOUR EMAIL ADDRESS
SUPPORT_EMAIL_PASSWORD = ""         # YOUR EMAIL PASSWORD

# SQL SETTINGS                      # LEAVE AS IS
TABLE_USER_LENGTH = 14
TABLE_CLIENTLOG_LENGTH = 6
TABLE_BLACKLIST_LENGTH = 4
TABLE_CONNECTUSERCOOKIES_LENGTH = 2
TABLE_COOKIES_LENGTH = 2
TABLE_DATA_LENGTH = 10
TABLE_SERVERLOG_LENGTH = 4

# 2FA SETTINGS                      # TIME IN SECONDS UNTIL 2FA CODES EXPIRE
ACCOUNT_ACTIVATION_MAX_TIME = 3600
DELETE_ACCOUNT_CONFIRMATION_MAX_TIME = 1800
NEW_DEVICE_CONFIRMATION_MAX_TIME = 1800
NEW_DEVICE_CONFIRMATION_ADMIN_MAX_TIME = 1800
PASSWORD_CHANGE_CONFIRMATION_MAX_TIME = 1800
PASSWORD_CHANGE_CONFIRMATION_ADMIN_MAX_TIME = 1800

# VERSION INFO                      # LEAVE AS IS
CONFIG_VERSION = "0.11-2.18"
CONFIG_BUILD = "development"
