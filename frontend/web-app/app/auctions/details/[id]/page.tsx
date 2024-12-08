import { getDetailedViewData } from "@/app/actions/auctionActions";
import Heading from "@/app/components/Heading";
import React from "react";
import CountdownTimer from "../../CountdownTimer";
import Carimage from "../../CarImage";
import DetailedSpecs from "./DetailedSpecs";
import { getCurrentUser } from "@/app/actions/authAuctions";
import EditButton from "./EditButton";
import DeleteButton from "../DeleteButton";
import BidList from "./BidList";
import { PageProps } from "@/.next/types/app/page";

export default async function Details (props: PageProps){
  const { params } = props;
  const { id } = await params;
  const data = await getDetailedViewData(id);
  const user = await getCurrentUser();
  
  return (
    <div>
      <div className="flex justify-between">
        <div className="flex items-center gap-3">
          <Heading title={`${data.make} ${data.model}`} />
          {user?.username === data.seller && (
            <>
              <EditButton id={data.id} />
              <DeleteButton id={data.id} />
            </>
          )}
        </div>
        <div className="flex gap-3">
          <h3 className="text-2xl font-semibold">Time remaining:</h3>
          <CountdownTimer auctionEnd={data.auctionEnd} />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-6 mt-3 ">
        <div className="relative w-full bg-gray-200 aspect-[16/10] rounded-lg overflow-hidden ">
          <Carimage imageUrl={data.imageUrl} />
        </div>
        <BidList user={user} auction={data} />
      </div>

      <div className="mt-3 grid grid-cols-1 rounded-lg">
        <DetailedSpecs auction={data} />
      </div>
    </div>
  );
}
