# Информация об отправителе в сообщениях чата

## Обновление: Добавлена информация об отправителе

В событиях создания сообщений (`message.created`) и в API эндпоинте получения сообщений (`GET /api/Chat/{chatId}/messages`) теперь включена полная информация об отправителе. Это особенно важно для **групповых чатов**, где нужно отображать имя и аватар отправителя.

---

## Изменения в API

### 1. Эндпоинт получения сообщений

**Эндпоинт:** `GET /api/Chat/{chatId}/messages?take=50&cursor={cursor}`

**Новая структура `MessageDTO`:**
```typescript
interface MessageDTO {
  id?: number;
  createdAt?: string;              // ISO 8601
  senderId?: number;
  senderName?: string;              // ✨ НОВОЕ: Имя отправителя
  senderAvatarImageId?: number;     // ✨ НОВОЕ: ID аватара отправителя
  senderPublicIdentifier?: string; // ✨ НОВОЕ: Публичный идентификатор
  textContent: string;
  replyMessageId?: number;
  readBy?: MessageReadDTO[];
}
```

**Пример ответа:**
```json
{
  "success": true,
  "message": "Messages retrieved successfully",
  "data": {
    "items": [
      {
        "id": 123,
        "createdAt": "2024-01-15T10:30:00Z",
        "senderId": 456,
        "senderName": "John Doe",
        "senderAvatarImageId": 789,
        "senderPublicIdentifier": "john-doe",
        "textContent": "Hello everyone!",
        "replyMessageId": null,
        "readBy": []
      }
    ],
    "nextCursor": "124"
  }
}
```

### 2. Изменения в `MessageCreatedEvent` (SignalR)

### Новая структура события:

```typescript
interface MessageCreatedEvent {
  id: number;                    // ID сообщения
  chatId: number;                 // ID чата
  senderId: number;               // ID отправителя (как было раньше)
  senderName: string;             // ✨ НОВОЕ: Имя отправителя
  senderAvatarImageId: number;    // ✨ НОВОЕ: ID аватара отправителя
  senderPublicIdentifier: string; // ✨ НОВОЕ: Публичный идентификатор отправителя
  textContent: string;            // Текст сообщения
  replyMessageId?: number;        // ID сообщения, на которое отвечают (если есть)
  createdAtUtc: string;           // Время создания (ISO 8601)
}
```

### Что изменилось:

**Добавлены поля:**
- `senderName` - Имя пользователя (UserName или "FirstName LastName")
- `senderAvatarImageId` - ID изображения аватара
- `senderPublicIdentifier` - Публичный идентификатор для ссылок на профиль

**Старые поля остались без изменений:**
- `senderId` - по-прежнему доступен для обратной совместимости

---

## Примеры использования

### JavaScript/TypeScript с SignalR

```typescript
// Подключение к SignalR Hub
const connection = new signalR.HubConnectionBuilder()
  .withUrl("/hubs/chat", {
    accessTokenFactory: () => getAuthToken()
  })
  .build();

// Обработка события создания сообщения
connection.on("message.created", (event: MessageCreatedEvent) => {
  console.log("New message received:", event);
  
  // Для групповых чатов - отображаем имя и аватар отправителя
  if (isGroupChat(event.chatId)) {
    displayMessage({
      id: event.id,
      text: event.textContent,
      sender: {
        id: event.senderId,
        name: event.senderName,              // Используем новое поле
        avatarImageId: event.senderAvatarImageId, // Используем новое поле
        publicIdentifier: event.senderPublicIdentifier, // Для ссылки на профиль
        avatarUrl: `/api/file/image/${event.senderAvatarImageId}` // Генерация URL
      },
      createdAt: new Date(event.createdAtUtc),
      replyTo: event.replyMessageId
    });
  } else {
    // Для личных чатов имя не обязательно, но можно использовать
    displayMessage({
      id: event.id,
      text: event.textContent,
      sender: {
        id: event.senderId,
        name: event.senderName, // Опционально для личных чатов
        avatarImageId: event.senderAvatarImageId
      },
      createdAt: new Date(event.createdAtUtc)
    });
  }
});
```

### React компонент

```tsx
import { useEffect } from 'react';
import { useSignalR } from './useSignalR';

interface Message {
  id: number;
  text: string;
  sender: {
    id: number;
    name: string;
    avatarImageId: number;
    publicIdentifier: string;
  };
  createdAt: Date;
}

export function ChatMessages({ chatId, isGroup }: { chatId: number; isGroup: boolean }) {
  const { connection } = useSignalR();
  const [messages, setMessages] = useState<Message[]>([]);

  useEffect(() => {
    if (!connection) return;

    const handler = (event: MessageCreatedEvent) => {
      if (event.chatId !== chatId) return;

      const newMessage: Message = {
        id: event.id,
        text: event.textContent,
        sender: {
          id: event.senderId,
          name: event.senderName,
          avatarImageId: event.senderAvatarImageId,
          publicIdentifier: event.senderPublicIdentifier
        },
        createdAt: new Date(event.createdAtUtc)
      };

      setMessages(prev => [...prev, newMessage]);
    };

    connection.on("message.created", handler);

    return () => {
      connection.off("message.created", handler);
    };
  }, [connection, chatId]);

  return (
    <div className="messages">
      {messages.map(msg => (
        <MessageBubble
          key={msg.id}
          message={msg}
          showSenderName={isGroup} // Показываем имя только в групповых чатах
        />
      ))}
    </div>
  );
}

function MessageBubble({ message, showSenderName }: { message: Message; showSenderName: boolean }) {
  const avatarUrl = `/api/file/image/${message.sender.avatarImageId}`;
  const profileUrl = `/user/${message.sender.publicIdentifier}`;

  return (
    <div className="message-bubble">
      {showSenderName && (
        <div className="message-sender">
          <img src={avatarUrl} alt={message.sender.name} className="avatar" />
          <a href={profileUrl} className="sender-name">
            {message.sender.name}
          </a>
        </div>
      )}
      <div className="message-text">{message.text}</div>
      <div className="message-time">
        {message.createdAt.toLocaleTimeString()}
      </div>
    </div>
  );
}
```

### Vue.js пример

```vue
<template>
  <div class="chat-messages">
    <div
      v-for="message in messages"
      :key="message.id"
      class="message"
    >
      <!-- Для групповых чатов показываем имя и аватар -->
      <div v-if="isGroupChat" class="message-header">
        <img
          :src="getAvatarUrl(message.sender.avatarImageId)"
          :alt="message.sender.name"
          class="avatar"
        />
        <a
          :href="`/user/${message.sender.publicIdentifier}`"
          class="sender-name"
        >
          {{ message.sender.name }}
        </a>
      </div>
      <div class="message-content">{{ message.text }}</div>
      <div class="message-time">{{ formatTime(message.createdAt) }}</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { useSignalR } from '@/composables/useSignalR';

interface MessageCreatedEvent {
  id: number;
  chatId: number;
  senderId: number;
  senderName: string;
  senderAvatarImageId: number;
  senderPublicIdentifier: string;
  textContent: string;
  replyMessageId?: number;
  createdAtUtc: string;
}

const props = defineProps<{
  chatId: number;
  isGroupChat: boolean;
}>();

const { connection } = useSignalR();
const messages = ref<Message[]>([]);

const handleMessageCreated = (event: MessageCreatedEvent) => {
  if (event.chatId !== props.chatId) return;

  messages.value.push({
    id: event.id,
    text: event.textContent,
    sender: {
      id: event.senderId,
      name: event.senderName,
      avatarImageId: event.senderAvatarImageId,
      publicIdentifier: event.senderPublicIdentifier
    },
    createdAt: new Date(event.createdAtUtc)
  });
};

const getAvatarUrl = (imageId: number) => {
  return `/api/file/image/${imageId}`;
};

onMounted(() => {
  if (connection) {
    connection.on('message.created', handleMessageCreated);
  }
});

onUnmounted(() => {
  if (connection) {
    connection.off('message.created', handleMessageCreated);
  }
});
</script>
```

---

## Генерация URL для аватара

Для получения изображения аватара используйте эндпоинт:

```
GET /api/file/image/{avatarImageId}
```

Пример:
```typescript
const avatarUrl = `/api/file/image/${event.senderAvatarImageId}`;
// или
const avatarUrl = `https://your-api-domain.com/api/file/image/${event.senderAvatarImageId}`;
```

---

## Ссылки на профиль пользователя

Используйте `senderPublicIdentifier` для создания ссылок на профиль:

```typescript
const profileUrl = `/user/${event.senderPublicIdentifier}`;
// или
const profileUrl = `https://your-app.com/user/${event.senderPublicIdentifier}`;
```

---

## Обратная совместимость

Старое поле `senderId` остается доступным для обратной совместимости. Если ваше приложение еще не обновлено, вы можете продолжать использовать `senderId` и делать дополнительные запросы для получения информации о пользователе:

```typescript
// Старый способ (все еще работает, но не рекомендуется)
if (!event.senderName) {
  // Fallback: загрузить информацию о пользователе по senderId
  const user = await fetchUserById(event.senderId);
  displayMessage({
    sender: {
      id: event.senderId,
      name: user.name,
      avatarImageId: user.avatarImageId
    },
    // ...
  });
}
```

**Рекомендация:** Обновите код для использования новых полей `senderName`, `senderAvatarImageId` и `senderPublicIdentifier` - это избавит от необходимости делать дополнительные запросы к API.

---

## Когда показывать имя отправителя

- **Групповые чаты (`IsGroup = true`)**: Всегда показывайте имя и аватар отправителя
- **Личные чаты (`IsGroup = false`)**: Имя отправителя опционально (обычно не показывается, так как в чате только два участника)

Пример проверки:

```typescript
// При получении информации о чате
const chat = await fetchChatInfo(chatId);
const isGroupChat = chat.isGroup;

// При обработке сообщения
if (isGroupChat) {
  // Показываем имя и аватар
  displaySenderInfo(message.sender);
}
```

---

## Обновление существующего кода

Если у вас уже есть обработка события `message.created`, обновите код следующим образом:

### Было:
```typescript
connection.on("message.created", (event) => {
  // Нужно было делать дополнительный запрос для получения имени
  fetchUserById(event.senderId).then(user => {
    displayMessage({
      sender: {
        id: event.senderId,
        name: user.name, // Дополнительный запрос
        avatar: user.avatar
      },
      text: event.textContent
    });
  });
});
```

### Стало:
```typescript
connection.on("message.created", (event) => {
  // Вся информация уже в событии!
  displayMessage({
    sender: {
      id: event.senderId,
      name: event.senderName,              // ✨ Без дополнительных запросов
      avatarImageId: event.senderAvatarImageId,
      publicIdentifier: event.senderPublicIdentifier
    },
    text: event.textContent
  });
});
```

---

## Типы TypeScript

Для удобства можете добавить типы:

```typescript
// types/chat.ts
export interface MessageCreatedEvent {
  id: number;
  chatId: number;
  senderId: number;
  senderName: string;
  senderAvatarImageId: number;
  senderPublicIdentifier: string;
  textContent: string;
  replyMessageId?: number;
  createdAtUtc: string;
}

export interface MessageSender {
  id: number;
  name: string;
  avatarImageId: number;
  publicIdentifier: string;
}

export interface ChatMessage {
  id: number;
  text: string;
  sender: MessageSender;
  createdAt: Date;
  replyTo?: number;
}
```

---

---

## Использование в API запросах

### Получение сообщений с информацией об отправителе

```typescript
// Запрос сообщений
const response = await fetch(`/api/Chat/${chatId}/messages?take=50`, {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});

const data = await response.json();
const messages = data.data.items;

// Теперь в каждом сообщении есть информация об отправителе
messages.forEach(message => {
  console.log(`Message from ${message.senderName}`);
  console.log(`Avatar: /api/file/image/${message.senderAvatarImageId}`);
  console.log(`Profile: /user/${message.senderPublicIdentifier}`);
});
```

### Пример обработки в React

```tsx
function ChatMessages({ chatId }: { chatId: number }) {
  const [messages, setMessages] = useState<MessageDTO[]>([]);

  useEffect(() => {
    fetchMessages(chatId).then(setMessages);
  }, [chatId]);

  return (
    <div className="messages">
      {messages.map(msg => (
        <div key={msg.id} className="message">
          {/* Для групповых чатов показываем имя и аватар */}
          {msg.senderName && (
            <div className="message-sender">
              <img
                src={`/api/file/image/${msg.senderAvatarImageId}`}
                alt={msg.senderName}
              />
              <span>{msg.senderName}</span>
            </div>
          )}
          <div className="message-text">{msg.textContent}</div>
        </div>
      ))}
    </div>
  );
}
```

---

## Вопросы и поддержка

Если у вас возникли вопросы по использованию новой информации об отправителе, обращайтесь к команде backend разработки.
