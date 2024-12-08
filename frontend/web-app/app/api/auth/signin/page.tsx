import { PageProps } from "@/.next/types/app/page";
import EmptyFilter from "@/app/components/EmptyFilter";
import React from "react";

export default async function Signin(props: PageProps ) {
  const { searchParams } = props;
  const { callbackUrl } = await searchParams;
  return (
    <EmptyFilter 
        title='You need to be logged in to do that'
        subtitle='Please click below to login'
        showLogin
        callbackUrl={callbackUrl}/>
  );
}
