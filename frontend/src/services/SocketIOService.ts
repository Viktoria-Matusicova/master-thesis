import { io } from "socket.io-client";
import type Notification from "../interfaces/Notification";
import { useNotificationStore } from "../stores/notificationStore";
import Utility from "../utility/utility";
import { useRuleStore } from "../stores/ruleStore";

class SocketIOService {
  private URL = "private_url";

  //Initializes socket connection after user succesfull login.
  public initialize = (() => {
    let socket = io(this.URL, { secure: true });
    const store = useNotificationStore();
    const jwt = localStorage.getItem("access_token")!
    const registrationDate = Utility.getRegistrationDate(jwt);
    store.registrationDate = registrationDate;
    const ruleStore = useRuleStore();

    //Listens on successful connection to the socket server.
    socket.on("connect", () => {
      console.log("Client is successfully connected to socket io server");
      socket.emit("join", "rules");
      socket.emit("join", "rules:update");
      socket.on("disconnect", () => {
      });

      //Listens to socket room 'rules' to get notification data. If data are retrieved, it pushes notification which are newer than user's registration date to the store.
      //It leaves this room after that.
      socket.on("rules", (data: any[]) => {
        let notifications: Notification[] = [];
        data.forEach(element => {
          const notification = JSON.parse(element) as Notification;
          if (new Date(notification.date) > store.registrationDate!) {
            notifications.push(notification);
          }
        });
        if (notifications.length != 0) {
          store.notifications = notifications;
          socket.emit("leave", "rules");
        } else {
          store.notifications = [];
        }
      });

      //Listens to socket room 'rules:update' to get newest notification in real time. If data are retrieved, it pushes these notifications to the store.
      socket.on("rules:update", (data: string) => {
        if (Object.keys(data).length !== 0) {
          const notification = JSON.parse(data) as Notification;
          store.notifications.unshift(notification);
          ruleStore.getTreeNodes();
        }
      });
    });
  })
}

export default new SocketIOService();

