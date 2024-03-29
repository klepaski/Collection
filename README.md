# Collection
Веб-приложение .NET Core MVC для управления личными коллекциями (книги, марки и т.д.)
Авторизация с помощью Identity.
База данных MSSQL, EF, FluentAPI, миграции.
Асинхронные операции.
AJAX запросы на страницах с помощью jquery.
Для хранения картинок - облако DropBox, в БД хранятся ссылки на изображения.
Отображение лайков и комментариев динамически с помощью SignalR (по протоколу websocket)

<b>Администратор:</b>
* Управлять всеми пользователями: удалить, (раз)блокировать, выдать/забрать права администратора
* Управлять коллекциями всех пользователей, как их владелец
* Добавлять/удалять темы коллекций

<b>Пользователь:</b>
* Поставить лайк коллекции, оставить комментарий (Отображаются линейно без задержки у всех пользователей с помощью SignalR)
* Создать коллекцию: по желанию добавить фото (сохраняется в Dropbox), указать дополнительные поля айтемов, входящих в эту коллекцию (например, коллекция Книги, пользователь добавляет поля Год издания (datetime), Автор (string), Рейтинг книги (integer). Далее при добавлении новых книг в коллекцию необходимо указать эти поля, помимо обязательных у всех айтемов (название, тэги). Также выбирается тема коллекции из фиксированного списка.
* Добавить тэги коллекций (связь многие-ко-многим). Далее тэги отображаются на главной странице в виде облака тэгов.
* Изменить, удалить коллекцию. Аналогично для айтемов коллекций.

<b>Гость:</b>
* Просматривать коллекции и айтемы
* Использовать страницу поиска (MSSQL Fulltext search), результаты запроса - коллекции либо айтемы. Если строка запроса содержится в комментарии, отображается соответсвующий айтем с этим комментарием. Аналогично с тэгами.








<b>--------------ТЗ:---------------</b>

.NET: C#/ASP.NET Core MVC/Entity Framework/SQL Server/MySQL/PostgreSQL, Bootstrap
Необходимо реализоваь Web-приложения для управления личными коллекциями. 
Неаутентифицированные пользователи имеют доступ только в режиме чтения (они могут использовать поиск, но не могут создавать коллекции и айтемы, не могут оставлять комментарии и лайки).

Админка позволяет управлять пользователями — блокировать, разблокировать, удалять, добавлять в админы, удалять из админов (АДМИН МОЖЕТ ЗАБРАТЬ У СЕБЯ ПРАВА АДМИНА, это важно).
Админ видит все страницы как их автор (например, админ может открыть коллекцию другого пользователя и добавить в нее айтемы; по сути, админ является владельцем всех коллекций и всех айтемов).

Пользователи могут зарегистрироваться и аутентифицироваться через сайт.
Каждая страниц (в хидере сверху) предоставляет доступ по полнотектстовому поиску. Результаты — всегда айтемы (т.е. если текст найден в комментарии, вы показываете ссылку на айтем с комментариями, а не отдельный комментарий).
Если результат коллекция, вы можете или показать любой айтем или же сгенерировать ссылку на коллекцию.

У каждого юзера есть личная страница, на которой он управляет своими коллекциями (создает, удаляет, редактирует) — каждая коллекция в списке это ссылка на страницу коллекции, которая содержит таблицу айтемов с сортировками и фильтрами и возможностью создать новый айтем, удалить или отредактировать существующий.

Каждая коллекция имеет название, описание, тему, опционального изображения (загружается пользователем в облако).

Дополнительно коллеуия позволяет указать поля, которые будут у каждого айтема. Плюс у айтемов есть фиксированные поля (id, название, тэги). На уровне коллекции для айтемов можно выбрать любой набор из следующих доступных: 3 целочисленных поля, 3 строковый поля, 3 многострочных текста, 3 логических да/нет чекбокса, 3 поля даты. Для каждого из выбранных полей пользователь задает название. 

Все айтемы должны иметь тэги (пользователь может ввести несколько тэгов; необходимо поддерживать автодополнние — когда пользователь начинает что-то вводить, ему показывается список из тэгов с соответствующими начальными буквами из тех, что уже есть в базе данных).

На главной странице:
-Список последних добавленных айтемов (имя, коллекция, автор);
-Список 5 самых больших коллекций;
-Облако тэгов (когда пользователь кликает на тэг, отображается список айтемов — в общем, лучше всего заюзать страницу результатов поиска).

Когда айтем открыт для просмотра (автором или другим пользователем) внизу отображаются комментарии. Комментарии линейные, добавляются всегда только в конец (нельзя откомментить комментарий в середине). Комментарии обновляются автоматически — когда страница открыта и кто-то другой добавил комментарий, он должен появиться автомагически (возможна задержка в 2-5 секунд).

Каждый айтем также содежрит лайки (не более одного от одного пользователя на каждый айтем).
Сайт должен поддерживать два языка: английский и еще один — польский, узбекский, грузинский, на выбор (пользователь выбирает и выбор сохраняется для пользователя).
Сайт должен поддерживать две визуальные темы (skins): светлую и темную (пользователь выбирает и выбор сохраняется).

Обязательно:
-Использовать CSS-фреймворк, например, Bootstrap (но можно любой другой);
-Поддерживать разные разрешения экрана (в том числе мобилки), адаптивная верстка;
-Использовать ORM для доступа к данным (sequelize, prism, typeorm, anything you like), 
Использовтаь движок полнотекстового поиска (или внешнюю либу или встроенные фичи базы) — нельзя реализовывать поиск через сканирование базы SELECT-ами.


