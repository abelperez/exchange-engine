<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"   
								xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
								xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">

<xsl:variable name="NEWLINE">
</xsl:variable>
	
	<!-- Context -->
	<xsl:variable name="ALX_VERTICAL" select="/soap:Envelope/soap:Body/ExchangeMessage/@vertical"/>
	<xsl:variable name="ALX_LEAD_STATUS" select="/soap:Envelope/soap:Body/ExchangeMessage/@status"/>

	<!-- Lead -->
	<xsl:variable name="ALX_LEAD_GUID" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/guid"/>
	<xsl:variable name="ALX_LEAD_AID" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/aid"/>
	<xsl:variable name="ALX_LEAD_CID" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/cid"/>
	<xsl:variable name="ALX_LEAD_TID" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/tid"/>
	<xsl:variable name="ALX_LEAD_SID" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/sid"/>
	<xsl:variable name="ALX_LEAD_FIRSTNAME" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/firstname"/>
	<xsl:variable name="ALX_LEAD_LASTNAME" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/lastname"/>
	<xsl:variable name="ALX_LEAD_EMAIL" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/email"/>
	<xsl:variable name="ALX_LEAD_PHONE" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/phone"/>
	<xsl:variable name="ALX_LEAD_SECONDARYPHONE" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/secondaryphone"/>
	<xsl:variable name="ALX_LEAD_STREET" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/street"/>
	<xsl:variable name="ALX_LEAD_CITY" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/city"/>
	<xsl:variable name="ALX_LEAD_ZIP" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/zipcode"/>
	<xsl:variable name="ALX_LEAD_STATE" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/state"/>
	<xsl:variable name="ALX_LEAD_CREATED" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/created"/>
	
	<!-- Debt Fields -->
	<xsl:variable name="ALX_LEAD_DEBT_CREDITCARD_AMOUNT" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/debt/creditcardamount"/>
	<xsl:variable name="ALX_LEAD_DEBT_MONTHLY_PAYMENTS" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/debt/monthlypayments"/>
	<xsl:variable name="ALX_LEAD_DEBT_PAYMENT_STATUS" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/debt/paymentstatus"/>
	<xsl:variable name="ALX_LEAD_DEBT_UNSECURED_AMOUNT" select="/soap:Envelope/soap:Body/ExchangeMessage/dataset[@id='']/lead/debt/unsecuredamount"/>

	<!-- Data Sets -->
	<xsl:variable name="ALX_RUNTIME_STRATEGIES" select="/soap:Envelope/soap:Header/action"/>

</xsl:stylesheet>