
( Constants )
0 Value fd-in
0 Value fd-out

256 Constant  /line-buf
Create line-buf  /line-buf allot
Create LF 10 c,

( Variables )
Variable s \ start of current column
Variable c  \ current char position
Variable e \ end (addr + length) of current line

( File Operations )
: open-input       ( addr u -- )  r/o open-file   throw  to fd-in ;
: open-output      ( addr u -- )  w/o create-file throw  to fd-out ;
: open-input-file  ( -- )  s" input.csv" open-input ;
: open-output-file ( -- )  s" docs/output.md"  open-output ;
: open-files       ( -- )  open-input-file  open-output-file ;
: close-input      ( -- )  fd-in  close-file throw ;
: close-output     ( -- )  fd-out close-file throw ;
: close-files      ( -- )  close-input close-output ;

: readln      ( -- u flag ) line-buf  /line-buf fd-in read-line throw ;
: write-bytes ( a u -- ) fd-out write-file throw ;
: nl          ( -- ) LF 1 write-bytes ;

: c<e   ( -- flag )  c @ e @ u< ;
: c==,  ( -- flag )  c @ c@ [char] , = ;
: c++   ( -- )       c @ 1+ c ! ;

: init-vars ( u -- ) line-buf swap over + e ! dup s ! c ! ;

: |>    ( -- )        s" | " write-bytes ;
: wcol  ( -- )        s @ c @ over - write-bytes ;
: scanln ( -- )  begin  c<e 0= if exit then  c==, if exit then c++ again ;
: eol        ( -- )  e @ c ! ;
: nextcolpos ( -- )  c @ 1+ dup s ! c ! ;
: header ( -- addr u ) S\" ---\ntitle: Operational Contacts\nsidebar_position: 10\n---\n\n# AO Core Operational Contacts\n\n" ;
: file-header ( -- ) header write-bytes ;
: write-header   ( u -- n ) init-vars |>  0 >r begin c<e while scanln wcol r> 1+ >r c<e if |> nextcolpos else |> eol then repeat nl r> ;
: write-separator ( n -- )  |> 0 ?do s" ---|" write-bytes loop nl ;
: exit-if-empty ( u flag -- ) 0= if drop exit then ;
: write-data-row ( u -- ) init-vars |> begin c<e while scanln wcol c<e if |> nextcolpos else |> eol then repeat nl ;
: create-body   ( -- ) begin readln while write-data-row repeat drop ;
: create-header ( u -- ) file-header write-header write-separator ;

( Main )
: csv>md   ( -- )  readln exit-if-empty create-header create-body ;
: transform ( -- ) open-files csv>md close-files ;
