# Серверная часть LiteCall

![Project Image](https://raw.githubusercontent.com/Htomsik/Htomsik/main/Assets/collage.png)


---

## __Оглавление__


- [Описание](#Описание)
- [Серверная часть LiteCall](#Серверная-часть-LiteCall "Code-Nostra")
- [Клиентская часть LiteCall](#Клиентская-часть-LiteCall "Htomsik")
- [Возможности приложения](#Что-может-делать-приложение?)
- [Ссылки на авторов](#Ссылки-на-авторов)

---

## __Описание__

Создание  мессенджера LiteCall стало экспериментом из разряда: сможем ли мы за месяц сделать курсовую работу такого уровня?

На данный момент мессенджер реализован на связке ***WPF+SignalR Core+ASP.NET*** 

На первом этапе работы из-за недостатка знаний в работе межсетевых приложений был выбран фреймворк WCF который не очень подходить для нестатических задачь
т.к. нужно обрабатывать все исключительные случаи (обрывы соединения, тайм-ауты и т.д)

При проверке в глобальной сети столкнулись с серьезными проблемами тайм-аута( когда вы хотите использовать дуплексный сервис в течение длительного времени). Нам пришлось периодически совершать сервисные вызовы, чтобы поддерживать клиентские соединения.

В дипломной работе WCF будет заменен на SignalR Core, так как он проще лично для нас работает с потоковой передачей данных чем WCF.

### __11.03.2022__ Первая  рабочая SignalR версия клиента

Выпущена первая рабочая версия SignalR клиента, на ней отсуствуют пролемы которые были на WCF.

### __Окно авторизации__

![Project Image](https://github.com/Code-Nostra/LiteCall-Servers/blob/master/ReameAssets/Login.png?raw=true)

### __Окно подключения к серверу__

![Project Image](https://github.com/Code-Nostra/LiteCall-Servers/blob/master/ReameAssets/Main.png?raw=true)

### __Страница сервера__

![Project Image](https://github.com/Code-Nostra/LiteCall-Servers/blob/master/ReameAssets/ServerRoom.png?raw=true)

### __Диаграмма классов клиента__

![Project Image](https://raw.githubusercontent.com/Htomsik/LiteCall/master/ReadmeAssets/ClassDiagram1.png)



### __Окно сервера__

![Project Image](https://github.com/Code-Nostra/LiteCall-Servers/blob/master/ReameAssets/ServerConsol.png)

#### Технологии

#### **WCF версия**
- .Net
- WPF
- WCF

#### **SignalR версия**

- .Net
- WPF
- ASP.NET Core SignalR


#### Паттерн

- mvvm

### nuget пакеты клиента
- NAudio
- System.Windows.Interactivity.WPF

### nuget пакеты сервера авторизации
- Entity Framework
- Microsoft.AspNetCore.Authentication.JwtBearer
- System.Windows.Interactivity.WPF

### nuget пакеты сервера чата
- Microsoft.AspNetCore.Authentication.JwtBearer
- System.Text.JsonF

## __Серверная часть LiteCall__
Серверная часть чата реализованна на ASP.NET Core SignalR, т.к. по сравнению с ASP.NET signalr он поддерживает
потоковую передачу данных и более новее.

**Задачи которые реализованны Сервером чата([SignalRServer](https://github.com/Code-Nostra/LiteCall-Servers/tree/master/SignalRServer)):**
- Валидация JWT
- Обмент текстовые сообщениями
- Голосовой чат(стабильная работа с 2-мя пользователями)
- Создание комнат(групп) для общения
- Автоматическое удаление пустых групп при отключении пользователей
- Автоматическое удаление при из групп при отключении
- Котроль имён пользователей(уникальность имён)

**Стабильная работа с любыми возникшими непредвиденными обстоятельствами**

****

**Задачи которые реализованны Сервером Авторизации([ServerAuthorization](https://github.com/Code-Nostra/LiteCall-Servers/tree/master/ServerAuthorization)):**
- Авторизация посредством JWT
- Шифрование JWT токена с помошью RSA256
- Регистрация пользователя
- Получения списка серверов и информации о них

<details>
<summary>Подробнее про авторизацию</summary>
<br>
Пароль при передаче шифруются однонаправленным алгоритмом SHA-1, целостность передачи важных данных гарантирует Json Web Token. Json Web Token зашифрован с помощью алгоритма SHA-256


** **
**Конструкция JWT используемая нами**

![Project Image](https://github.com/Code-Nostra/LiteCall-Servers/blob/master/ReameAssets/JWT.png)
</details>

## __Клиентская часть LiteCall__



## __Установка__

[Ссылка на последнюю версию установщика WCF клиента]() 
>Установщик временно отсуствует

[Ссылка на установщик серверов]() 
>Установщик временно отсуствует

---
## __Недобавленные возможности/баги:__
- Невозможность подключения больше чем 2 человек в голосовой чат
- Отсутсвие нормального отключения пользователя от сервера __[11.03.22 исправлено]__
- Отсуствие нормальной валидации __[10.03.22 исправлено]__
- Отсуствие нормального тестирования
---
## __Ссылки на авторов__

Клиентская часть:

[![Костя](https://img.shields.io/badge/-Костя-1C1C22?style=for-the-badge&logo=vk&logoColor=blue)](https://vk.com/jessnjake)


Серверная часть:

[![Артём](https://img.shields.io/badge/-Артём-1C1C22?style=for-the-badge&logo=vk&logoColor=red)](https://vk.com/id506987182)


