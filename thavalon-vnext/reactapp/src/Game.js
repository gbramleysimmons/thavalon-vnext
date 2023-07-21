import React, { Component } from 'react';
import Typography from '@mui/joy/Typography';
import Input from '@mui/joy/Input';
import Button from '@mui/joy/Button';
class Game extends Component {
    constructor(props) {
        super(props);
        this.state = { selectedPlayer : "" }
    }

    componentDidMount() {
        this.setState({ ready: true });
    }

    renderPlayerList() {
        return Object.entries(this.props.game.roles).map(([key, val]) => <Button onClick={() => this.setSelectedPlayer(key)}> { key }</Button >)
    }

    renderRole(playerInfo) {
        console.log(playerInfo);
        return <div> Role {playerInfo.roleName} Alignment: {playerInfo.alignment} Information: {playerInfo.information.map(ele => <li>{ele}</li>)} </div>
    }
    setSelectedPlayer(player) {
        this.setState({ selectedPlayer : player })
    } 
    render() {
        let content = this.state.selectedPlayer === "" ? this.renderPlayerList() : <div> {this.renderRole(this.props.game.roles[this.state.selectedPlayer])} <Button onClick={() => this.setSelectedPlayer("")}>Back</Button> </div>
        return (
            <div>
                {this.state.ready ? <div>
                    {content}
                    <Button onClick={() => this.props.exitGame()}>Exit Game</Button>
            </div>
                    : <div> Loading... </div>}
            </div>
        );
    }

}
export default Game;
