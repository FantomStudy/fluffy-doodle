Сейчас схема такая.

Роуты Unity
Точка входа одна:
- /practice/play

Уровень выбирается query-параметром scene:
- /practice/play?scene=if -> Level1
- /practice/play?scene=variables -> Level2
- /practice/play?scene=level3 -> Level3
- /practice/play?scene=cycles -> Level4

Поддерживаются и алиасы:
- scene=level1
- scene=level2
- scene=level3
- scene=level4

Мобильный режим:
- mobile=1

Примеры:
- десктоп: /practice/play?scene=cycles
- mobile/react-native webview: /practice/play?scene=cycles&mobile=1

Что делает `mobile=1`
- включает мобильный canvas UI_Canvas_StarterAssetsInputs_Joysticks
- показывает мобильные кнопки
- добавляет кнопку интеракта
- после завершения уровня редиректит на фронт :8080/courses

Без mobile=1:
- считается обычный web-фронт
- редирект после уровня идёт на :5173/courses

Как работает completion
После финиша уровня:
1. идёт затемнение
2. во время затемнения отправляется completion request на бэк
3. показывается popup
4. через 5 секунд идёт редирект

Endpoint completion:
- POST /game/levels/level_1/complete
- POST /game/levels/level_2/complete
- POST /game/levels/level_3/complete
- POST /game/levels/level_4/complete

По умолчанию API идёт:
- на тот же хост
- но на порт 3000

То есть если Unity открыт на http://myhost:9000/practice/play?scene=cycles, completion уйдёт на:
- http://myhost:3000/game/levels/level_4/complete

При желании можно явно переопределить:
- api=http://host:3000
- или backend=http://host:3000

Редирект после уровня
Приоритет такой:
1. если передан redirect=..., идёт туда
2. иначе если передан frontend=..., идёт туда + /courses
3. иначе:
- без mobile=1 -> тот же хост на :5173/courses
- с mobile=1 -> тот же хост на :8080/courses

Как открывать с кнопки на сайте
Для обычного сайта:
window.location.href = "/practice/play?scene=if";
window.location.href = "/practice/play?scene=variables";
window.location.href = "/practice/play?scene=level3";
window.location.href = "/practice/play?scene=cycles";

Для mobile/webview:
window.location.href = "/practice/play?scene=cycles&mobile=1";

Если надо явно задать API:
window.location.href = "/practice/play?scene=cycles&api=http://localhost:3000";

Как интегрировать билд с nginx
Обычно WebGL билд содержит:
- index.html
- папку Build
- папку TemplateData

Их можно положить, например, в:
- /var/www/unity/

Тогда nginx может раздавать Unity по /practice/play.

Пример конфига:
```
server {
    listen 80;
    server_name your-domain.com;

    location = /practice/play {
        root /var/www/unity;
        try_files /index.html =404;
    }

    location /practice/Build/ {
        alias /var/www/unity/Build/;
    }

    location /practice/TemplateData/ {
        alias /var/www/unity/TemplateData/;
    }

    location ~* \.js\.br$ {
        add_header Content-Type application/javascript;
        add_header Content-Encoding br;
    }

    location ~* \.wasm\.br$ {
        add_header Content-Type application/wasm;
        add_header Content-Encoding br;
    }

    location ~* \.data\.br$ {
        add_header Content-Type application/octet-stream;
        add_header Content-Encoding br;
    }

    location ~* \.js\.gz$ {
        add_header Content-Type application/javascript;
        add_header Content-Encoding gzip;
    }

    location ~* \.wasm\.gz$ {
        add_header Content-Type application/wasm;
        add_header Content-Encoding gzip;
    }

    location ~* \.data\.gz$ {
        add_header Content-Type application/octet-stream;
        add_header Content-Encoding gzip;
    }
}
```
Но для Unity WebGL правильнее ещё сделать rewrite, чтобы /practice/play?... отдавал index.html, а ассеты шли отдельно. Более надёжный вариант:
server {
    listen 80;
    server_name your-domain.com;

    location = /practice/play {
        alias /var/www/unity/index.html;
    }

    location /practice/Build/ {
        alias /var/www/unity/Build/;
    }

    location /practice/TemplateData/ {
        alias /var/www/unity/TemplateData/;
    }

    location ~* \.js\.br$ {
        default_type application/javascript;
        add_header Content-Encoding br;
    }

    location ~* \.wasm\.br$ {
        default_type application/wasm;
        add_header Content-Encoding br;
    }

    location ~* \.data\.br$ {
        default_type application/octet-stream;
        add_header Content-Encoding br;
    }
}

Как лучше в проде
Если у вас несколько контейнеров:
- frontend
- backend
- unity static files

то обычно наружу ставят один nginx, который:
- / -> фронт
- /practice/play и /practice/Build/* -> Unity WebGL
- /api или другой route -> backend

Если хотите, следующим сообщением я могу дать вам уже готовый полный nginx.conf под вашу схему:
- фронт 5173
- react native web 8080
- backend 3000
- unity по /practice/play