# Collection
Веб-приложение .NET Core MVC для управления личными коллекциями (книги, марки и т.д.)
Авторизация с помощью Identity
База данных MSSQL, EF, FluentAPI, миграции.
Асинхронные операции.
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
