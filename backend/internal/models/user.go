package models

import "gorm.io/gorm"

type User struct {
	gorm.Model
	Login        string `gorm:"not null;unique;" json:"login"`
	Password     string `gorm:"not null" json:"-"`
	FullName     string `gorm:"not null" json:"fullName"`
	PhoneNumber  string `gorm:"not null" json:"phoneNumber"`
	RefreshToken string `json:"refreshToken"`
}

// регаются учителя. у них есть ссылка, которую даёт пиздюку. он регается, фио, логин пароль, телефон
