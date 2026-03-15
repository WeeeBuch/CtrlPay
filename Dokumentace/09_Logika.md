# Logika
V aplikaci jsou 4 různé role:
Customer - dostávají uživatelé patřící stálým zákazníkům
Accountant - účetní, spravuje platby a zákazníky
Admin - spravuje uživatele
Employee - to be done

Základní účel aplikace je práce s platbami v Moneru, jejich správa a evidence. Základem jsou tedy samozřejmě platby, ty mohou nabývat několika hlavních stavů - Pending, WaitingForPayment, Paid, Overpaid, Expired viz dále. 

Abychom mohli vytvořit platbu, musíme mít zákazníka. V našem případě stálého zákazníka (Loyal customer), běžní zákazníci mohou platit přes platební bránu, která zatím není implementovaná. U zákazníka evidujeme všechny běžné údaje a jakmile je uznán za stálého((promotuje ho účetní), dostane svůj vlastní account v monero peněžence.

Vytvoříme tedy platbu a ta se ve výchozím nastaví na pending a zákazník ji vidí, když se přihlásí do aplikace. Poté má dvě možnosti jak platbu zaplatit:
1) Z kreditu - kredit se dá získat, buď dobitím na hlavní adresu accountu zákazníka(získá v aplikaci), nebo tím, že nějakou z předchozích plateb přeplatí a přeplatek se převede na kredit.
2) Pomocí platby na jednorázovou adresu - v aplikaci klikne na tuto možnost a je mu vygenerována nová monero account adresa kterou použije k zaplacení
Zde mohou nastat možnosti:
	- Zaplatí málo - platba se nastaví na PartiallyPaid
	- Zaplatí moc - platba se nastaví na Overpaid
	- Zaplatí správně - platba se nastaví na Paid
Přeplatí-li zákazník platbu, má účetní možnost převést přeplatek na kredit do aplikace.
Vyprší-li lhůta k platbě, nastaví ji jeden z procesů na pozadí na Expired.
