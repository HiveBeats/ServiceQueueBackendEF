// You can edit this code!
// Click here and start typing.
package main

import (
	"bytes"
	"encoding/json"
	"log"
	"net/http"
	"os"
)

type User struct {
	UserName string
	Email    string
	Password string
}

func getFilledUser() User {
	usr := User{os.Getenv("FILLR_User__UserName"), os.Getenv("FILLR_User__Email"), os.Getenv("FILLR_User__Password")}

	return usr
}

func main() {
	url := os.Getenv("FILLR_Connection__WebApi")

	log.Printf("UserFiller starts with %s", url)

	usr := getFilledUser()
	payload, err := json.Marshal(usr)
	if err != nil {
		log.Fatal(err)
		return
	}

	log.Printf("UserFiller requested...")
	resp, err := http.Post(url + "Auth/Register", "application/json", bytes.NewBuffer(payload))
	if err != nil {
		log.Fatal(err)
		return
	}

	if resp.StatusCode >= 200 && resp.StatusCode <= 299 {
		log.Printf("UserFiller requested successfully")
	} else {
		log.Printf("UserFiller requested unsuccessfully")
	}

	log.Printf("UserFiller closing")
}