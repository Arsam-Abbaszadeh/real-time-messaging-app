### auth logic

ok so have auth business logic in the auth store
for the auth store use the setup Store not the options store, its the recommended and more flexible API.
keep the request logic seperetted and use that in the store actions themselves, they should be cleanely able to be async.
have the login, create acc, etc methods return result objects, with error messages and success and what not.

prevent login attempt if loading, either with spinning circle, is true or just block input edits or at least the input button

### AFTER SUCCESFUL LOGIN

someone enters username and password and we should direct them to the main chat page, but not any specific chat page.
should that be /chat and then specific chats are /chat/chat-id, each chat has a unique ID of course and so we can easily block someone from entering.
If they are succesfully logged in

login, request was succesful, redirect to another page

### Login wheel

as the for the login wheel, may be able to make it a global object to just import around and toggle on and off programittacly, worse case it can just be a div we make toggle visibility for.
Toggling it on an off should be delt with by the component we navigate to, not the one before.
Toggles should really be used for network requests prioring to rendering something, not minor changes in HTML or CSS, that should be instant anyway.

### Message aknowledgements and stuff

I think we may want to create a toast or something in general for making acknoledgements of specific things.
Like a little message shows up from the top right saying succesful login or something
