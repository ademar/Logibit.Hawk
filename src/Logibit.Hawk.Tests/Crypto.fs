﻿module Logibit.Hawk.Tests.Crypto

open Expecto
open NodaTime
open Logibit.Hawk
open Logibit.Hawk.Types
open Logibit.Hawk.Tests.Shared

[<Tests>]
let crypto =
  testList "crypto" [
    testCase "valid normalized string" <| fun _ ->
      let subject =
        Crypto.genNormStr
          "header"
          { credentials = credentials SHA256 
            timestamp   = Instant.FromSecondsSinceUnixEpoch 1357747017L
            nonce       = "k3k4j5"
            ``method``  = GET
            resource    = "/resource/something"
            host        = "example.com"
            port        = 8080us
            ext         = None
            hash        = None
            app         = None
            dlg         = None }
      Expect.equal subject
                   "hawk.1.header\n1357747017\nk3k4j5\nGET\n/resource/something\nexample.com\n8080\n\n\n"
                   "should return a valid normalized string"

    testCase "valid normalized string (app + no dlg)" <| fun _ ->
      let subject =
        Crypto.genNormStr
          "header"
          { credentials = credentials SHA256
            timestamp   = Instant.FromSecondsSinceUnixEpoch 1357747017L
            nonce       = "k3k4j5"
            ``method``  = GET
            resource    = "/resource/something"
            host        = "example.com"
            port        = 8080us
            ext         = None
            hash        = None
            app         = Some "app"
            dlg         = None }
      Expect.equal subject
                   "hawk.1.header\n1357747017\nk3k4j5\nGET\n/resource/something\nexample.com\n8080\n\n\napp\n\n"
                   "should return a valid normalized string"

    testCase "valid normalized string (ext)" <| fun _ ->
      let subject =
        Crypto.genNormStr
          "header"
          { credentials =
              { id        = "sample"
                key       = "dasdfasdf"
                algorithm = SHA256 }
            timestamp   = Instant.FromSecondsSinceUnixEpoch 1357747017L
            nonce       = "k3k4j5"
            ``method``  = GET
            resource    = "/resource/something"
            host        = "example.com"
            port        = 8080us
            ext         = Some "this is some data"
            hash        = None
            app         = None
            dlg         = None }
      Expect.equal subject
                   "hawk.1.header\n1357747017\nk3k4j5\nGET\n/resource/something\nexample.com\n8080\n\nthis is some data\n"
                   "should return a valid normalized string"

    testCase "valid normalized string (payload + ext)" <| fun _ ->
      let subject =
        Crypto.genNormStr
          "header"
          { credentials =
              { id        = "sample"
                key       = "dasdfasdf"
                algorithm = SHA256 }
            timestamp   = Instant.FromSecondsSinceUnixEpoch 1357747017L
            nonce       = "k3k4j5"
            ``method``  = GET
            resource    = "/resource/something"
            host        = "example.com"
            port        = 8080us
            ext         = Some "this is some data"
            hash        = Some "U4MKKSmiVxk37JCCrAVIjV/OhB3y+NdwoCr6RShbVkE="
            app         = None
            dlg         = None }
      Expect.equal subject
                   "hawk.1.header\n1357747017\nk3k4j5\nGET\n/resource/something\nexample.com\n8080\nU4MKKSmiVxk37JCCrAVIjV/OhB3y+NdwoCr6RShbVkE=\nthis is some data\n"
                   "should return a valid normalized string"

    testCase "normalised payload hash" <| fun _ ->
      let payload = Some (UTF8.bytes "description=a&timestamp=2015-01-06T00%3A00%3A00.000Z&amount=12&currency=SEK&targets%5Beconomic%5D%5Bkey%5D=economic&targets%5Beconomic%5D%5Btitle%5D=Economic+Finance+Voucher&receipt_id=6cf8a352bc16439ca60895da7d0dfadb")
      let contentType = Some "application/x-www-form-urlencoded; charset=UTF-8"
      let result = Crypto.calcPayloadHashString payload Algo.SHA256 contentType
      Expect.equal result "SRNdUbnjvHd/UVk2Strp7EA3hLNQMjOOh2FPH4MSlBI=" "correct payload hash"
  ]