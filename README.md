_Projekt ist derzeit leider eingeschlafen. :-(_

# Gesetzesentwicklung-Tools 

## Über dieses Projekt 

Dieses Projekt, bestehend aus den drei Teilprojekten 
Gesetzesentwicklung, Gesetzesentwicklung-Daten und 
Gesetzesentwicklung-Tool, stellt einen Demonstrator zu den 
Möglichkeiten von Open Data im Bereich der Gesetzgebung dar. 

### Problem 

Die Gesetzgebung in Deutschland genügt nicht den Kriterien von Open 
Data, da diese eine strukturierte und maschinenlesbare Form der 
bereitgestellten Dokumente verlangt. 

#### Drucksachen des Bundestags 

Die vom Bundestag in erster, zweiter und dritter Lesung behandelten 
Gesetzesentwürfe sind als Drucksachen zwar über die 
[Parlamentsdokumentation](http://pdok.bundestag.de/) recherchierbar, das 
hierbei genutzte Format *PDF* ist jedoch nicht maschinell auswertbar. 

#### Bundesgesetzblatt 

Das von der *Bundesanzeiger Verlagsgesellschaft mbH* vertriebene 
Bundesgesetzblatt bereitet ebenfalls Probleme; so stehen zwar wenigstens 
alle seit 1949 erschienen Bundesgesetzblätter kostenfrei zur 
Verfügung, deren Verwendung ist jedoch stark eingeschränkt: 

1. Die seit 1998 erschienen PDFs des Bundesgesetzblattes scheinen aus 
einem Satz-Programm heraus erstellt worden zu sein, so dass die in den 
PDFs enthaltenen Texte daher der maßgeblichen Print-Version entsprechen 
sollten. Sie sind jedoch nicht nutzbar: Die PDFs sind kennwortgeschützt 
und verhindern sowohl das Drucken als auch das Kopieren der darin 
enthaltenen Inhalte. 

2. Die PDF-Ausgaben des Bundesgesetzblattes vor 1998 stellen Scans der 
jeweiligen Print-Versionen dar und enthalten keine Texte. Auch diese 
PDFs sind per Kennwort vor einer Weiterverarbeitung, wie dem Einsatz 
einer Texterkennung (OCR), geschützt. 

3. Die Nutzungslizenz ist fragwürdig: 

> Die elektronische Version des Bundesgesetzblattes genießt generell 
Datenbankschutz nach §§ 87a ff UrhG. Dies bezieht sich auch auf die 
einzelne Ausgabe des Bundesgesetzblattes, die deshalb nicht ohne 
Zustimmung des Verlages außerhalb der gesetzlichen Vorschriften 
genutzt werden darf. Eine unveränderte Weiterverwendung entnommener 
pdf-Dateien im Original, die über den privaten Gebrauch hinausgeht, ist 
daher nicht statthaft. 

#### Bundesanzeiger 

Ebenfalls von der *Bundesanzeiger Verlagsgesellschaft mbH* wird der 
Bundesanzeiger vertrieben. Probleme hierbei: 

1. Erst seit 1. April 2012 steht der Bundesanzeiger in Gänze digital 
zur Verfügung. Zwischen 2002 und 2012 gabe es parallel zum 
Bundesanzeiger (BAnz) noch den elektronischen Bundesanzeiger (eBAnz). 
Der gedruckte Bundesanzeiger gibt es nicht in einer digitalisierten 
Version. Jedenfalls konnte ich nichts finden ... 

2. Fragwürdige 
[Nutzungslizenz](https://www.bundesanzeiger.de/ebanzwww/i18n/doc//D052.p 
df?document=D34&language=de): 

> Es ist dem Kunden nur gestattet, die von ihm aus den Daten entnommenen 
Informationen auszudrucken. Ein darüber hinausgehendes
Publikationsrecht steht dem Kunden genauso wenig zu, wie die von ihm 
ausgedruckten Informationen zu vervielfältigen, abzuändern, zu 
verbreiten, nachzudrucken, dauerhaft zu speichern, insbesondere zum 
Aufbau einer Datenbank zu verwenden oder an D ritte weiterzugeben. Alle 
ihm zustehenden Urheberrechte behält sich der Bundesanzeiger Verlag 
vor. 

Der Kennwortschutz von gegängelten PDFs lässt sich glücklicherweise 
relativ einfach umgehen. 

Was die Lizenzproblematiken angeht, berufe ich mich für dieses Projekt 
auf die [Antwort](http://dipbt.bundestag.de/doc/btd/17/093/1709374.pdf) 
der Bundesregierung zur Kleinen Anfrage *Urheberrechtliche Situation, 
Open Data und offene Lizenzen bei Dokumenten und Inhalten der 
Bundesregierung*. Insbesondere verweise ich auf die Antwort zur Frage 
13. 

## Build status
[![Build status][appveyor_build_badge]][appveyor_build_link]
[![Test status][appveyor_tests_badge]][appveyor_tests_link]
[![Coverity Scan Build Status][coverity_badge]][coverity_link]
[![Coverage Status][coveralls_badge]][coveralls_link] 

[appveyor_build_badge]: https://ci.appveyor.com/api/projects/status/pgegkkstup8r6sh6?svg=true 
[appveyor_build_link]: https://ci.appveyor.com/project/chkpnt/gesetzesentwicklung-tools 
[appveyor_tests_badge]: http://teststatusbadge.azurewebsites.net/api/status/chkpnt/gesetzesentwicklung-tools 
[appveyor_tests_link]: https://ci.appveyor.com/project/chkpnt/gesetzesentwicklung-tools/build/tests
[coverity_badge]: https://scan.coverity.com/projects/4507/badge.svg 
[coverity_link]: https://scan.coverity.com/projects/4507 
[coveralls_badge]: https://coveralls.io/repos/chkpnt/Gesetzesentwicklung-Tool/badge.svg 
[coveralls_link]: https://coveralls.io/r/chkpnt/Gesetzesentwicklung-Tool 
